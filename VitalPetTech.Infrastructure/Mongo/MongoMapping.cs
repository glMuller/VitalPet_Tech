using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Infrastructure.Mongo;

// Mapeamento feito aqui para o Domain continuar sem nenhuma dependência do driver
public static class MongoMapping
{
    private static readonly object Lock = new();
    private static bool _registered;

    public static void Register()
    {
        lock (Lock)
        {
            if (_registered) return;

            BsonClassMap.RegisterClassMap<LeituraSensor>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(x => x.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
                cm.GetMemberMap(x => x.Tipo).SetSerializer(new EnumSerializer<TipoAlerta>(BsonType.String));
                cm.GetMemberMap(x => x.Timestamp).SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
            });

            _registered = true;
        }
    }
}