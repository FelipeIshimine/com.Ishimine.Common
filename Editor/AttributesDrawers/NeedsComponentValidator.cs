using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor.Validation;
using UnityEngine;

[assembly:RegisterValidator(typeof(NeedsComponentValidator))]
public class NeedsComponentValidator : AttributeValidator<NeedsComponentAttribute, GameObject>
{
    protected override void Validate(ValidationResult result)
    {
        if(this.ValueEntry.SmartValue == null)
            return;

        if (this.ValueEntry.SmartValue.GetComponent(this.Attribute.type) == null)
        {
            result.ResultType = ValidationResultType.Error;
            result.Message = $"This needs a {this.Attribute.type.Name}";
        }
    }
}
