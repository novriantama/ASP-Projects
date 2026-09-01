namespace ASPProjects.Business.Services;

public interface IIdProtector
{
    string Encode(int id);
    int Decode(string encodedId);
    bool TryDecode(string encodedId, out int id);
}
