using Terraria.Audio;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Systems;

public class AudioSystem : ModSystem
{
    public static SoundStyle ReturnSound(string sound, float pitchVariance = 0.2f, float volume = 1f)
    {
        SoundStyle soundStyle = new($"DartGunsPlus/Audio/{sound}")
        {
            Volume = volume,
            PitchVariance = pitchVariance
        };

        return soundStyle;
    }
}