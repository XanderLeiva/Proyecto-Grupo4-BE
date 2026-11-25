using Proyecto_Grupo4_BE.Models;

namespace Proyecto_Grupo4_BE.Services;

public class Farmacia
{
    public List<Medicamento> Medicamentos { get; set; } = new List<Medicamento>();
    public List<Inventario> Inventarios { get; set; } = new List<Inventario>();
    public List<Receta> Recetas { get; set; } = new List<Receta>();
    public List<Entrega> Entregas { get; set; } = new List<Entrega>();

    private int _siguienteMedicamentoId = 1;
    private int _siguienteInventarioId = 1;
    private int _siguienteRecetaId = 1;
    private int _siguienteEntregaId = 1;

    public void AgregarMedicamento(Medicamento medicamento)
    {
        medicamento.Id = _siguienteMedicamentoId.ToString();
        _siguienteMedicamentoId++;

        medicamento.FechaRegistro = DateTime.UtcNow;

        Medicamentos.Add(medicamento);

        Console.WriteLine($"Medicamento agregado: {medicamento.Nombre} (ID: {medicamento.Id})");
    }

    public void ListarMedicamentos()
    {
        Console.WriteLine("Lista de medicamentos registrados");

        foreach (var medicamento in Medicamentos)
        {
            Console.WriteLine($"[{medicamento.Id}] {medicamento.Nombre}");
            Console.WriteLine($"     Dosis: {medicamento.Dosis}");
            Console.WriteLine($"     Descripcion: {medicamento.Descripcion}");
            Console.WriteLine($"     Fecha: {medicamento.FechaRegistro}");
            Console.WriteLine();
        }
    }

    public void AgregarStock
        (string idCentro, string idMedicamento, string lote, DateTime fechaVencimiento, int cantidad, int nivelMinimo)
    {
        var medicamento = Medicamentos.FirstOrDefault(m => m.Id == idMedicamento);

        if (medicamento == null)
        {
            Console.WriteLine("Error: medicamento no existe");
            return;
        }

        if (cantidad <= 0)
        {
            Console.WriteLine("Error: cantidad debe ser mayor a 0");
            return;
        }

        var inventario = Inventarios.FirstOrDefault(i =>
            i.IdCentro == idCentro && i.IdMedicamento == idMedicamento && i.Lote == lote);

        if (inventario != null)
        {
            inventario.Stock += cantidad;
        }
        else
        {
            inventario = new Inventario();
            inventario.Id = _siguienteInventarioId.ToString();
            _siguienteInventarioId++;
            inventario.IdCentro = idCentro;
            inventario.IdMedicamento = idMedicamento;
            inventario.Lote = lote;
            inventario.FechaVencimiento = fechaVencimiento;
            inventario.Stock = cantidad;
            inventario.NivelMinimo = nivelMinimo;

            Inventarios.Add(inventario);
        }

        Console.WriteLine("Stock actualizado");
    }

    // Agregar receta
    public void AgregarReceta(Receta receta)
    {
        var medicamento = Medicamentos.FirstOrDefault(m => m.Id == receta.IdMedicamento);

        if (medicamento == null)
        {
            Console.WriteLine("Error: medicamento no existe");
            return;
        }

        if (receta.Cantidad <= 0)
        {
            Console.WriteLine("Error: cantidad debe ser mayor a 0");
            return;
        }

        receta.Id = _siguienteRecetaId.ToString();
        _siguienteRecetaId++;

        receta.Entregado = 0;
        receta.Estado = "Pendiente";
        receta.FechaRegistro = DateTime.UtcNow;

        Recetas.Add(receta);

        Console.WriteLine($"Receta agregada (ID: {receta.Id}) del paciente {receta.IdPaciente}");
    }

    public Receta? BuscarRecetaPorId(string id)
    {
        return Recetas.FirstOrDefault(r => r.Id == id);
    }

    public void ListarRecetas()
    {
        Console.WriteLine("Lista de recetas registradas");

        foreach (var receta in Recetas)
        {
            Console.WriteLine($"[{receta.Id}] Paciente: {receta.IdPaciente}");
            Console.WriteLine($"     Medicamento ID: {receta.IdMedicamento}");
            Console.WriteLine($"     Cantidad: {receta.Cantidad}");
            Console.WriteLine($"     Entregado: {receta.Entregado}");
            Console.WriteLine($"     Estado: {receta.Estado}");
            Console.WriteLine();
        }
    }

    public void EntregarMedicamento(string recetaId, string inventarioId, string idCentro, int cantidad)
    {
        var receta = BuscarRecetaPorId(recetaId);

        if (receta == null)
        {
            Console.WriteLine("Error: receta no existe");
            return;
        }

        var inventario = Inventarios.FirstOrDefault(i => i.Id == inventarioId);

        if (inventario == null)
        {
            Console.WriteLine("Error: inventario no existe");
            return;
        }

        if (receta.IdCentro != idCentro || inventario.IdCentro != idCentro)
        {
            Console.WriteLine("Error: solo el personal del centro puede entregar");
            return;
        }

        if (cantidad <= 0 || cantidad > receta.Cantidad - receta.Entregado)
        {
            Console.WriteLine("Error: cantidad invalida o mayor a lo pendiente de la receta");
            return;
        }

        if (cantidad > inventario.Stock)
        {
            Console.WriteLine("Error: no hay lo suficiente en stock");
            return;
        }

        inventario.Stock -= cantidad;
        receta.Entregado += cantidad;

        if (receta.Entregado == receta.Cantidad)
        {
            receta.Estado = "Completa";
        }
        else
        {
            receta.Estado = "Parcial";
        }

        var entrega = new Entrega();
        entrega.Id = _siguienteEntregaId.ToString();
        _siguienteEntregaId++;
        entrega.IdReceta = recetaId;
        entrega.IdInventario = inventarioId;
        entrega.Cantidad = cantidad;
        entrega.FechaRegistro = DateTime.UtcNow;

        Entregas.Add(entrega);

        Console.WriteLine("Entrega registrada");
    }
}
