using System.ComponentModel.DataAnnotations;

namespace ICG.NetCore.Utilities.Tests;

public enum TestEnum
{
    CleanValue = 0,

    [Display(Name = "Testing")]
    FormattedValue = 1
}