using Tel.Egram.Services.Persistance.Resources;

namespace Tel.Egram.Services.Persistance;

// ToDo: If this really is a "Resource Manager", it should be expanded to encompass all Resources
public interface IResourceManager
{
    IList<PhoneCode> GetPhoneCodes();
}