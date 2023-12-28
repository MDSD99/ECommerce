﻿using System.Reflection;


#nullable disable

namespace OrderService.Domain.SeedWork
{
    public class Enumeration : IComparable
    {
        public string Name { get; private set; }

        public int Id { get; private set; }

        protected Enumeration(int id, string name) => (Id, Name) = (id, name);

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
            typeof(T).GetFields(
            BindingFlags.Public |
            BindingFlags.Static |
            BindingFlags.DeclaredOnly).Select(s=>s.GetValue(null)).Cast<T>();

        public override bool Equals(object obj)
        {
            if (obj is not Enumeration otherValue)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches; 
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static int AbsoluteDifference(Enumeration firstValue,Enumeration secondValue)
        => Math.Abs(firstValue.Id - secondValue.Id);

        public static T FromValue<T>(int value) where T : Enumeration
        => Parse<T, int>(value, "value", item => item.Id == value);

        public static T FromDisplayName<T>(string displayname) where T : Enumeration
        => Parse<T, string>(displayname, "display name", item => item.Name == displayname);

        private static T Parse<T,K>(K value,string description,Func<T,bool> predicate) where T:Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);
            if (matchingItem == null)
            {
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");
            }
            return matchingItem;
        }

        public int CompareTo(object obj) => Id.CompareTo(((Enumeration)obj).Id);
    }
}

