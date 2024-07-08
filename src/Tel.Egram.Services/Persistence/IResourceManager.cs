using Tel.Egram.Services.Persistence.Resources;

namespace Tel.Egram.Services.Persistence;

// ToDo: If this really is a "Resource Manager", it should be expanded to encompass all Resources
public interface IResourceManager
{
    IList<PhoneCode> GetPhoneCodes();
}