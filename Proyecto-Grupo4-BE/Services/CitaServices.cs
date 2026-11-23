namespace Proyecto_Grupo4_BE.Services;

public class CitaServices
{
     private readonly FirebaseService _firebaseService;
    
    //inyeccion de dependencias
    public CitaServices(FirebaseService firebaseService)
    {
        _firebaseService = firebaseService;
    }


    private static Dictionary<string, object> ADiccionario(Cita cita)
    {
        return new Dictionary<string, object>
        {
            { "Id", cita.Id },
            { "IdPaciente", cita.IdPaciente },
            { "IdCentro", cita.IdCentro },
            { "Servicio", cita.Servicio },
            { "FechaCita", cita.FechaCita },
            { "EstadoCita", cita.EstadoCita },
            { "MotivoEstado", cita.MotivoEstado },
            { "FechaRegistro", cita.FechaRegistro },
            { "FechaModificacion", cita.FechaModificacion },
            { "ActualizadaPor", cita.ActualizadaPor }
        };
    }

    private static Cita MapearCita(DocumentSnapshot doc)
    {
        var data = doc.ToDictionary();

        return new Cita
        {
            Id = data["Id"].ToString()!,
            IdPaciente = data["IdPaciente"].ToString()!,
            IdCentro = data["IdCentro"].ToString()!,
            Servicio = data["Servicio"].ToString()!,
            FechaCita = data["FechaCita"].ToString()!,
            EstadoCita = data["EstadoCita"].ToString()!,
            MotivoEstado = data["MotivoEstado"].ToString()!,
            
            FechaRegistro = ((Timestamp)data["FechaRegistro"]).ToDateTime(),
            FechaModificacion = ((Timestamp)data["FechaModificacion"]).ToDateTime(),
            ActualizadaPor = data["ActualizadaPor"].ToString()!
        };
    }

    private async Task<Cita> ObtenerPorId(string idCita)
    {
        var snapshot = await _firebaseService.GetCollection("cita")
            .WhereEqualTo("Id", idCita)
            .GetSnapshotAsync();
        
        if(snapshot.Count == 0)
          throw new Exception("La cita no esxiste");
        
        return MapearCita(snapshot.Documents[0]);
    }

    private async Task<DocumentSnapshot?> BuscarCupo(string idCentro, string servicio, string fecha)
    {
        var snapshot = await _firebaseService.GetCollection("cupo")
            .WhereEqualTo("IdCentro", idCentro)
            .WhereEqualTo("Servicio", servicio)
            .WhereEqualTo("Fecha", fecha)
            .GetSnapshotAsync();

        if (snapshot.Count == 0)
            return null;
        
        return snapshot.Documents[0];
    }

    private static int LeerNumero(DocumentSnapshot cupo, string campo)
    {
        var data = cupo.ToDictionary();
        return int.Parse(data[campo].ToString()!);
    }

    private async Task ActualizarOcupados(DocumentSnapshot cupo, int nuevoValor)
    {
        var data = cupo.ToDictionary();
        data["Ocupados"] = nuevoValor;

        await _firebaseService.GetCollection("cupo")
            .Document(cupo.Id)
            .SetAsync(data);
    }

    private async Task LiberarCupo(string idCentro, string servicio, string fecha)
    {
        var cupo = await BuscarCupo(idCentro, servicio, fecha);

        if (cupo == null)
            return;
        
        var ocupados = LeerNumero(cupo, "Ocupados");
        
        if (ocupados > 0)
            await ActualizarOcupados(cupo, ocupados - 1);
    }
    
    //el paciente solicita una cita
    public async Task<Cita> Solicitar(CitaDto dto, string idPaciente)
    {
        //buscar el cupo en la fecha solicitada
        var cupo = await BuscarCupo(dto.IdCentro, dto.Servicio, dto.Fecha);

        if (cupo == null)
            throw new Exception("No hay cupos definidos en esa fecha");

        //validar que haya cupos
        var total = LeerNumero(cupo, "Total");
        var ocupados = LeerNumero(cupo, "Ocupados");

        if (ocupados >= total)
            throw new Exception("Ya se agotaron los cupos de ese dia");
        
        //validar que el usuario no tenga citas duplicadas
        var activas = await _firebaseService.GetCollection("cita")
            .WhereEqualTo("IdPaciente", idPaciente)
            .WhereEqualTo("Servicio", dto.Servicio)
            .WhereEqualTo("EstadoCita", EstadosCita.Programada)
            .GetSnapshotAsync();

        if (activas.Count > 0)
        {
            var existente = MapearCita((activas.Documents[0]));
            throw new Exception("Ya tiene una cita para ese servicio, programada el " + existente.FechaCita);
        }
        
        //crear una cita
        var cita = new Cita()
        {
            Id = Guid.NewGuid().ToString(),
            IdPaciente = idPaciente,
            IdCentro = dto.IdCentro,
            Servicio = dto.Servicio,
            FechaCita = dto.Fecha,
            EstadoCita = EstadosCita.Programada,
            MotivoEstado = string.Empty,
            FechaRegistro = DateTime.UtcNow,
            FechaModificacion = DateTime.UtcNow,
            ActualizadaPor = idPaciente
        };
        
        //guardar la cita en firebase
        await _firebaseService.GetCollection("cita")
            .Document(cita.Id)
            .SetAsync(ADiccionario(cita));
        
        //marcar el cupo como ocupado
        await ActualizarOcupados(cupo, ocupados + 1);
        
        return cita;
    }
    
    //el paciente puede ver sus citas
    public async Task<List<Cita>> ObtenerMisCitas(string idPaciente)
    {
        var snapshot = await _firebaseService.GetCollection("cita")
            .WhereEqualTo("IdPaciente", idPaciente)
            .GetSnapshotAsync();

        var citas = new List<Cita>();

        foreach (var doc in snapshot.Documents)
        {
          citas.Add(MapearCita(doc));  
        }
        
        return citas;
    }
    
    //para que el personal vea las citas agendadas del dia 
    public async Task<List<Cita>> ObtenerAgenda(string idCentro, string fecha)
    {
        var snapshot = await _firebaseService.GetCollection("cita")
            .WhereEqualTo("IdCentro", idCentro)
            .WhereEqualTo("FechaCita", fecha)
            .GetSnapshotAsync();

        var citas = new List<Cita>();

        foreach (var doc in snapshot.Documents)
        {
            citas.Add(MapearCita(doc));  
        }
        
        return citas;
    }
    
    //para cancelar citas, el personal con una justificacion obligatoria 
    public async Task<Cita> Cancelar(string idCita, CancelarCitaDto dto, string idUsuario, string rol, string idCentro)
    {
        var cita = await ObtenerPorId(idCita);

        if (rol == "Personal")
        {
            if (cita.IdCentro != idCentro)
                throw new Exception("Esta cita no pertenece a este centro");

            if (string.IsNullOrEmpty(dto.Justificacion))
                throw new Exception("Justificacion obligatoria");
            
            cita.MotivoEstado = dto.Justificacion;
        }
        else
        {
            if (cita.IdPaciente != idUsuario)
                throw new Exception("Esta cita no es suya");

            cita.MotivoEstado = "Cancelada por el paciente";
        }

        if (cita.EstadoCita != EstadosCita.Programada)
            throw new Exception("Solo se pueden cancelar citas ya programadas");
        
        //cambiar datos de cita
        cita.EstadoCita = EstadosCita.Cancelada;
        cita.FechaModificacion = DateTime.UtcNow;
        cita.ActualizadaPor = idUsuario;
        
        //guardar
        await _firebaseService.GetCollection("cita")
            .Document(cita.Id)
            .SetAsync(ADiccionario(cita));
        
        //liberar cupo
        await LiberarCupo(cita.IdCentro, cita.Servicio, cita.FechaCita);
        
        return cita;
    }
    
    //reprogramar cita
    public async Task<Cita> Reprogramar(string idCita, ReprogramarCitaDto dto, string idUsuario, string idCentro)
    {
        var cita = await ObtenerPorId(idCita);

        if (cita.IdCentro != idCentro)
            throw new Exception("Esta cita no pertenece a su centro");

        if (cita.EstadoCita != EstadosCita.Programada)
            throw new Exception("Solo puede repogramar citas ya programadas");

        if (string.IsNullOrEmpty(dto.Justificacion))
            throw new Exception("Justificacion obligatoria");

        if (cita.FechaCita == dto.NuevaFecha)
            throw new Exception("La nueva fecha no puede ser igual a la actual");
        
        //verificar que haya cupos en la nueva fecha
        var nuevoCupo = await BuscarCupo(cita.IdCentro, cita.Servicio, dto.NuevaFecha);

        if (nuevoCupo == null)
            throw new Exception("No hay cupos definidos en la nueva fecha");
        
        var ocupadosNuevo = LeerNumero(nuevoCupo, "Ocupados");
        
        if (ocupadosNuevo >= LeerNumero(nuevoCupo, "Total"))
            throw new Exception("Cupos en la nueva fecha ya estan agotados");
        
        //liberar cupo de fecha vieja 
        await LiberarCupo(cita.IdCentro, cita.Servicio, cita.FechaCita);
        
        //llenar cupo de nueva fecha
        await ActualizarOcupados(nuevoCupo, ocupadosNuevo + 1);
        
        //actualizar cita
        cita.FechaCita = dto.NuevaFecha;
        cita.MotivoEstado = dto.Justificacion;
        cita.FechaModificacion = DateTime.UtcNow;
        cita.ActualizadaPor = idUsuario;

        await _firebaseService.GetCollection("cita")
            .Document(cita.Id)
            .SetAsync(ADiccionario(cita));
        
        return cita;
    }
    
    //marcar cita como atendida/ no asistio
    public async Task<Cita> MarcarEstado(string idCita, string nuevoEstado, string idUsuario, string idCentro)
    {
        var cita = await ObtenerPorId(idCita);

        if (cita.IdCentro != idCentro)
            throw new Exception("Solo el personal del centro puede cambiar el estado de la cita");

        if (cita.EstadoCita != EstadosCita.Programada)
            throw new Exception("Solo se pueden atender citas programadas");

        cita.EstadoCita = nuevoEstado;
        cita.FechaModificacion = DateTime.UtcNow;
        cita.ActualizadaPor = idUsuario;

        await _firebaseService.GetCollection("cita")
            .Document(cita.Id)
            .SetAsync(ADiccionario(cita));
        
        return cita;
    }
}