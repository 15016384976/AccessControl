using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessControl.Models
{
    public class MemberModel
    {
        public string Area { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(Area))
            {
                throw new Exception("member's area should not be null");
            }
            else
            {
                MemberModelValidationFactory.GetMemberModelValidation(Area)?.Validate(this);
            }
            return true;
        }
    }

    public interface IMemberModelValidation
    {
        string Area { get; }
        void Validate(MemberModel model);
    }

    public class MemberModelValidationFactory
    {
        private static readonly List<IMemberModelValidation> _memberValidations = new List<IMemberModelValidation>
        {
            new EooMemberModelValidation(),
            new FooMemberModelValidation()
        };

        public static IMemberModelValidation GetMemberModelValidation(string area)
        {
            return _memberValidations.FirstOrDefault(v => v.Area == area);
        }
    }

    public class EooMemberModelValidation : IMemberModelValidation
    {
        public string Area => "Eoo";

        public void Validate(MemberModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                throw new Exception("Eoo member's email should not be null");
            }
        }
    }

    public class FooMemberModelValidation : IMemberModelValidation
    {
        public string Area => "Foo";

        public void Validate(MemberModel model)
        {
            if (string.IsNullOrEmpty(model.Phone))
            {
                throw new Exception("Foo member's phone should not be null");
            }
        }
    } 
}
