using System.Data.Common;
using Google.Cloud.Firestore;

namespace ProyectoG4BE.Services;

public class FirebaseService
{
    private readonly FirestoreDb _firestoreDb;

    public FirebaseService()
    {
        var credentialPath = Path.Combine(AppContext.BaseDirectory, "Config", "firebase-credentials.json");
        
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);
        
        _firestoreDb = FirestoreDb.Create("proyecto-grupo4-d8fc1");
    }
    
    public CollectionReference GetCollection (string collectionName)
    {
        return _firestoreDb.Collection(collectionName);
    }
}