
public interface ISaveable
{
    /*** Make classes with data you want to save inherit from this interface. ***/

    object SaveState();

    void LoadState(object state);
}
