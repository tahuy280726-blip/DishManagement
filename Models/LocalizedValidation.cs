using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DishManagement.Models;

/// <summary>Bắt buộc nhập - thông báo lấy theo ngôn ngữ hiện tại.</summary>
public class LRequiredAttribute : RequiredAttribute
{
    private readonly string _key;
    public LRequiredAttribute(string key) { _key = key; }
    public override string FormatErrorMessage(string name) => L.T(_key);
}

/// <summary>Giới hạn độ dài chuỗi - thông báo theo ngôn ngữ hiện tại.</summary>
public class LStringLengthAttribute : ValidationAttribute
{
    private readonly int _max;
    private readonly string _key;
    public LStringLengthAttribute(int max, string key) { _max = max; _key = key; }
    public override string FormatErrorMessage(string name) => L.T(_key, _max);
    public override bool IsValid(object value) => value == null || value.ToString().Length <= _max;
}

/// <summary>Giá trị số phải lớn hơn 0 (và không vượt quá max) - thông báo theo ngôn ngữ hiện tại.</summary>
public class LPositiveAttribute : ValidationAttribute
{
    private readonly string _key;
    private readonly double _max;
    public LPositiveAttribute(string key, double max = double.MaxValue) { _key = key; _max = max; }
    public override string FormatErrorMessage(string name) => L.T(_key);

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;
        var d = Convert.ToDouble(value, CultureInfo.InvariantCulture);
        if (d <= 0) return new ValidationResult(L.T(_key));
        if (d > _max) return new ValidationResult(L.T("val.tooLarge", _max.ToString("#,##0", CultureInfo.InvariantCulture)));
        return ValidationResult.Success;
    }
}
