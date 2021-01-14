using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Models
{

    public class Subscription
    {
        public string Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public Guid TenantId { get; set; }
        public string DisplayName { get; set; }
        public string State { get; set; }
        //public SubscriptionPolicies SubscriptionPolicies { get; set; }
        public string AuthorizationSource { get; set; }
        //public List<ManagedByTenant> ManagedByTenants { get; set; }
        //public Tags Tags { get; set; }
    }

    public class IdName
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IdName()
        {

        }


        public IdName(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            //return $"{Name}-Id:{Id}";
            return $"{Name}";
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(obj, this))
                return true;
            if (obj.GetType() != GetType())
                return false;

            IdName rhs = obj as IdName;
            return Id == rhs.Id && Name == rhs.Name;
        }

        public static bool operator ==(IdName lhs, IdName rhs) { return Equals(lhs, rhs); }
        public static bool operator !=(IdName lhs, IdName rhs) { return !Equals(lhs, rhs); }
    }
}
