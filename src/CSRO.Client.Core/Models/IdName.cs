using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Core.Models
{
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

        public override string ToString() => $"{Name}";

        /// <summary>
        /// Works in some case, need to relay on ToString
        /// </summary>
        public Func<IdName, string> IdNameConverter = p => p?.Name;

        #region GetHashCode and Equals

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

        #endregion
    }
}
