﻿namespace CSRO.Server.Domain
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
