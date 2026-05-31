using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Falling_Tokens_Game
{
    internal class GameMusic
    {
        private static Note[] backgroundMusic = {
    new Note(Tone.Mi, Duration.QUARTER),
    new Note(Tone.Si, Duration.EIGHTH),
    new Note(Tone.Do, Duration.EIGHTH),
    new Note(Tone.Re, Duration.QUARTER),
    new Note(Tone.Do, Duration.EIGHTH),
    new Note(Tone.Si, Duration.EIGHTH),
    new Note(Tone.La, Duration.QUARTER),
    new Note(Tone.La, Duration.EIGHTH),
    new Note(Tone.Do, Duration.EIGHTH),
    new Note(Tone.Mi, Duration.QUARTER),
    new Note(Tone.Re, Duration.EIGHTH),
    new Note(Tone.Do, Duration.EIGHTH),
    new Note(Tone.Si, Duration.QUARTER),
    new Note(Tone.Si, Duration.EIGHTH),
    new Note(Tone.Do, Duration.EIGHTH),
    new Note(Tone.Re, Duration.QUARTER),
    new Note(Tone.Mi, Duration.QUARTER),
    new Note(Tone.Do, Duration.QUARTER),
    new Note(Tone.La, Duration.QUARTER),
    new Note(Tone.La, Duration.HALF),
    new Note(Tone.Re, Duration.QUARTER),
    new Note(Tone.Re, Duration.EIGHTH),
    new Note(Tone.Fa, Duration.EIGHTH),
    new Note(Tone.La, Duration.QUARTER),
    new Note(Tone.Sol, Duration.EIGHTH),
    new Note(Tone.Fa, Duration.EIGHTH),
    new Note(Tone.Mi, Duration.QUARTER),
    new Note(Tone.Mi, Duration.EIGHTH),
    new Note(Tone.Do, Duration.EIGHTH),
    new Note(Tone.Mi, Duration.QUARTER),
    new Note(Tone.Re, Duration.EIGHTH),
    new Note(Tone.Do, Duration.EIGHTH),
    new Note(Tone.Si, Duration.QUARTER),
    new Note(Tone.Si, Duration.EIGHTH),
    new Note(Tone.Do, Duration.EIGHTH),
    new Note(Tone.Re, Duration.QUARTER),
    new Note(Tone.Mi, Duration.QUARTER),
    new Note(Tone.Do, Duration.QUARTER),
    new Note(Tone.La, Duration.QUARTER),
    new Note(Tone.La, Duration.HALF),
        };
        private static Note[] gameOverMusic =
        {
    new Note(Tone.SolD, Duration.EIGHTH),
    new Note(Tone.Re, Duration.EIGHTH),
    new Note(Tone.SolB, Duration.EIGHTH),
    new Note(Tone.Sol, Duration.EIGHTH),
    new Note(Tone.FaD, Duration.EIGHTH),
    new Note(Tone.Fa, Duration.EIGHTH),
    new Note(Tone.Mi, Duration.EIGHTH),
    new Note(Tone.ReD, Duration.EIGHTH),
    new Note(Tone.Re, Duration.EIGHTH),
    new Note(Tone.DoD, Duration.EIGHTH),
    new Note(Tone.Do, Duration.EIGHTH),
    new Note(Tone.Si, Duration.HALF),
        };

        public static void PlayMusic()
        {
            bool stopSong = false;
            while (!stopSong)
            {
                stopSong = Program.GameOver;

                Note[] song = Program.GameOver ? gameOverMusic : backgroundMusic;

                foreach (Note n in song)
                {
                    if (Program.GameOver != (song == gameOverMusic)) break;
                    else if (n.NoteTone == Tone.REST)
                        Thread.Sleep((int)n.NoteDuration);
                    else
                        Console.Beep((int)n.NoteTone, (int)n.NoteDuration);
                }
            }
        }
        protected enum Duration : int
        {
            WHOLE = 1600,
            HALF = WHOLE / 2,
            QUARTER = HALF / 2,
            EIGHTH = QUARTER / 2,
            SIXTEENTH = EIGHTH / 2,
        }
        protected enum Tone : int
        {
            REST = 0,
            SolB = 196,
            La = 220,
            LaD = 233,
            Si = 247,
            Do = 262,
            DoD = 277,
            Re = 294,
            ReD = 311,
            Mi = 330,
            Fa = 349,
            FaD = 370,
            Sol = 392,
            SolD = 415,
        }
        protected struct Note
        {
            Tone toneVal;
            Duration durationVal;


            public Note(Tone frequency, Duration time)
            {
                toneVal = frequency;
                durationVal = time;
            }

            public Tone NoteTone { get { return toneVal; } }
            public Duration NoteDuration { get { return durationVal; } }

        }



    }
}
