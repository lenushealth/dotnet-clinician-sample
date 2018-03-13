using System;
using Clinician.Units;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Clinician.TagHelpers
{
    [HtmlTargetElement("div", Attributes = SystolicAttributeName +"," +DiastolicAttributeName, TagStructure = TagStructure.NormalOrSelfClosing)]
    public class BloodPressureTagHelper : TagHelper
    {
        private const string DiastolicAttributeName = "health-diastolic";
        private const string SystolicAttributeName = "health-systolic";
        private const string UnitAttributeName = "health-unit";

        public BloodPressureTagHelper()
        {
            
        }

        [HtmlAttributeName(UnitAttributeName)] 
        public BloodPressureUnitEnum Unit { get; set; } = BloodPressureUnitEnum.mmHg;

        [HtmlAttributeName(DiastolicAttributeName)]
        public BloodPressureUnit Diastolic { get; set; }
        
        [HtmlAttributeName(SystolicAttributeName)]
        public BloodPressureUnit Systolic { get; set; }
        
        private IHtmlContent GetSystolicHtmlContentFragment()
        {
            var tag = new TagBuilder("span");
            tag.AddCssClass("label");
            tag.AddCssClass($"label-{this.CssClassForSystolicMeasure()}");
            tag.InnerHtml.AppendHtml(this.Systolic.ToString(this.Unit.ToString("G")));
            return tag;
        }

        private string CssClassForSystolicMeasure()
        {
            var value = this.Systolic.Category(BloodPressureUnits.Systolic);
            switch (value)
            {
                case BloodPressureUnitCategory.Low:
                    return "info";
                case BloodPressureUnitCategory.Normal:
                    return "success";
                case BloodPressureUnitCategory.PreHigh:
                    return "warning";
                case BloodPressureUnitCategory.High:
                    return "danger";
            }
            throw new NotSupportedException();
        }

        private string CssClassForDiastolicMeasure()
        {
            var value = this.Diastolic.Category(BloodPressureUnits.Diastolic);
            switch (value)
            {
                case BloodPressureUnitCategory.Low:
                    return "info";
                case BloodPressureUnitCategory.Normal:
                    return "success";
                case BloodPressureUnitCategory.PreHigh:
                    return "warning";
                case BloodPressureUnitCategory.High:
                    return "danger";
            }
            throw new NotSupportedException();
        }

        private IHtmlContent GetDiastolicHtmlContentFragment()
        {
            var tag = new TagBuilder("span");
            tag.AddCssClass("label");
            tag.AddCssClass($"label-{this.CssClassForDiastolicMeasure()}");
            tag.InnerHtml.AppendHtml(this.Diastolic.ToString(this.Unit.ToString("G")));
            return tag;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.AppendHtml(this.GetSystolicHtmlContentFragment()).AppendHtml(" / ").AppendHtml(this.GetDiastolicHtmlContentFragment());
            base.Process(context, output);
        }
    }
}
