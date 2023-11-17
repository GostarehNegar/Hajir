using System;
using System.Collections.Generic;
using System.Text;

namespace GN.Library.Xrm
{
    //[AttributeUsage(AttributeTargets.Property)]
    //
    // Summary:
    //     Specifies the inverse of a navigation property that represents the other end
    //     of the same relationship.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InversePropertyExAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.DataAnnotations.Schema.InversePropertyAttribute
        //     class using the specified property.
        //
        // Parameters:
        //   property:
        //     The navigation property representing the other end of the same relationship.
        public InversePropertyExAttribute(string property)
        {
            this.Property = property;
        }

        //
        // Summary:
        //     Gets the navigation property representing the other end of the same relationship.
        //
        // Returns:
        //     The property of the attribute.
        public string Property { get; }
    }

    //
    // Summary:
    //     Denotes a property used as a foreign key in a relationship.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ForeignKeyExAttribute : Attribute
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute
        //     class.
        //
        // Parameters:
        //   name:
        //     The name of the associated navigation property, or the name of one or more associated
        //     foreign keys.
        public ForeignKeyExAttribute(string name)
        {
            this.Name = name;
        }

        //
        // Summary:
        //     Gets the name of the associated navigation property or of the associated foreign
        //     keys.
        //
        // Returns:
        //     The name of the associated navigation property or of the associated foreign keys.
        public string Name { get; }
    }
}

