using System.Runtime.CompilerServices;
using QuestPDF.Infrastructure;

namespace ProjectTesting.HospitalTests;

public class QuestPdfTestSetup
{
    [ModuleInitializer]
    public static void Init()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }
}