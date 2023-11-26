using System.Collections;

public interface IPlayerPart
{
    public IEnumerator Main();
    public IEnumerator Reborn(bool ad);

    public IEnumerator Change();
    public IEnumerator Lose();

    public void SetupSkin(Skin skin);
}
