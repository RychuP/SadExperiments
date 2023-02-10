using System;
using System.IO;
using System.Threading;

namespace SadExperiments.Pages;

// C# version of a C program by Davide Della Casa
// which in turn is a de-obfuscated version of
// Yusuke Endoh' Fluid ascii simulation
//
// Unfortunately, my attempt at converting it to C#
// doesn't quite work... I'll come back to it at some point.
internal class Fluid : Page
{
    const int Gravity = 1;
    const int Pressure = 4;
    const int Viscosity = 7;
    const int CONSOLE_WIDTH = 80;
    const int CONSOLE_HEIGHT = 24;
    const int CharCount = CONSOLE_WIDTH * CONSOLE_HEIGHT;
    int xSandboxAreaScan = 0, ySandboxAreaScan = 0;
    byte[] screenBuffer = new byte[CharCount + 1];
    int[] wallflag = new int[CharCount * 2];
    double[] xPos = new double[CharCount * 2];
    double[] yPos = new double[CharCount * 2];
    double[] density = new double[CharCount * 2];
    double[] xForce = new double[CharCount * 2];
    double[] yForce = new double[CharCount * 2];
    double[] xVelocity = new double[CharCount * 2];
    double[] yVelocity = new double[CharCount * 2];
    double xParticleDistance, yParticleDistance;
    double particlesInteraction;
    double particlesDistance;
    int x, y, screenBufferIndex, totalOfParticles;
    
    public Fluid()
    {
        Title = "Fluid";
        Summary = "Ascii fluid simulation originally written by Yusuke Endoh.";
        Submitter = Submitter.Rychu;
        Tags = new Tag[]
        {

        };

        // read the input file to initialise the particles.
        // # stands for "wall", i.e. unmovable particles (very high density)
        // any other non-space character represents normal particles.
        int particlesCounter = 0;
        string source = File.ReadAllText("Resources/Fluid/Four.txt");
        foreach (char x in source)
        {
            switch (x)
            {
                case '\n':
                    // next row
                    // rewind the x to -1 cause it's gonna be incremented at the
                    // end of the while body
                    ySandboxAreaScan += 2;
                    xSandboxAreaScan = -1;
                    break;
                case ' ':
                    break;
                case '#':
                    // The character # represents “wall particle” (a particle with fixed position),
                    // and any other non-space characters represent free particles.
                    // A wall sets the flag on 2 particles side by side.
                    wallflag[particlesCounter] = wallflag[particlesCounter + 1] = 1;
                    break;
                default:
                    // Each non-empty character sets the position of two
                    // particles one below the other (real part is rows)
                    // i.e. each cell in the input file corresponds to 1x2 particle spaces,
                    // and each character sets two particles
                    // one on top of each other.
                    // It's as if the input map maps to a space that has twice the height, as if the vertical
                    // resolution was higher than the horizontal one.
                    // This is corrected later, see "y scale correction" comment.
                    // I think this is because of gravity simulation, the vertical resolution has to be
                    // higher, or conversely you can get away with simulating a lot less of what goes on in the
                    // horizontal axis.
                    xPos[particlesCounter] = xSandboxAreaScan;
                    yPos[particlesCounter] = ySandboxAreaScan;

                    xPos[particlesCounter + 1] = xSandboxAreaScan;
                    yPos[particlesCounter + 1] = ySandboxAreaScan + 1;

                    // we just added two particles
                    totalOfParticles = particlesCounter += 2;
                    break;
            }
            // next column
            xSandboxAreaScan += 1;
        }
    }

    public override void Update(TimeSpan delta)
    {
        int particlesCursor, particlesCursor2;

        // Iterate over every pair of particles to calculate the densities
        for (particlesCursor = 0; particlesCursor < totalOfParticles; particlesCursor++)
        {
            // density of "wall" particles is high, other particles will bounce off them.
            density[particlesCursor] = wallflag[particlesCursor] * 9;

            for (particlesCursor2 = 0; particlesCursor2 < totalOfParticles; particlesCursor2++)
            {

                xParticleDistance = xPos[particlesCursor] - xPos[particlesCursor2];
                yParticleDistance = yPos[particlesCursor] - yPos[particlesCursor2];

                particlesDistance = Math.Sqrt(Math.Pow(xParticleDistance, 2.0) + Math.Pow(yParticleDistance, 2.0));
                particlesInteraction = particlesDistance / 2.0 - 1.0;

                // this line here with the alternative test
                // works much better visually but breaks simmetry with the
                // next block
                //if (round(creal(particlesInteraction)) < 1){
                // density is updated only if particles are close enough
                if (Math.Floor(1.0 - particlesInteraction) > 0)
                {
                    density[particlesCursor] += particlesInteraction * particlesInteraction;
                }
            }
        }

        // Iterate over every pair of particles to calculate the forces
        for (particlesCursor = 0; particlesCursor < totalOfParticles; particlesCursor++)
        {
            yForce[particlesCursor] = Gravity;
            xForce[particlesCursor] = 0;

            for (particlesCursor2 = 0; particlesCursor2 < totalOfParticles; particlesCursor2++)
            {

                xParticleDistance = xPos[particlesCursor] - xPos[particlesCursor2];
                yParticleDistance = yPos[particlesCursor] - yPos[particlesCursor2];
                particlesDistance = Math.Sqrt(Math.Pow(xParticleDistance, 2.0) + Math.Pow(yParticleDistance, 2.0));
                particlesInteraction = particlesDistance / 2.0 - 1.0;

                // force is updated only if particles are close enough
                if (Math.Floor(1.0 - particlesInteraction) > 0)
                {
                    xForce[particlesCursor] += particlesInteraction * (xParticleDistance * (3 - density[particlesCursor] - density[particlesCursor2]) * Pressure + xVelocity[particlesCursor] *
                      Viscosity - xVelocity[particlesCursor2] * Viscosity) / density[particlesCursor];
                    yForce[particlesCursor] += particlesInteraction * (yParticleDistance * (3 - density[particlesCursor] - density[particlesCursor2]) * Pressure + yVelocity[particlesCursor] *
                      Viscosity - yVelocity[particlesCursor2] * Viscosity) / density[particlesCursor];
                }
            }
        }


        // empty the buffer
        Array.Clear(screenBuffer);

        for (particlesCursor = 0; particlesCursor < totalOfParticles; particlesCursor++)
        {

            if (wallflag[particlesCursor] != 0)
            {

                // This is the newtonian mechanics part: knowing the force vector acting on each
                // particle, we accelerate the particle (see the change in velocity).
                // In turn, velocity changes the position at each tick.
                // Position is the integral of velocity, velocity is the integral of acceleration and
                // acceleration is proportional to the force.

                // force affects velocity
                if (Math.Sqrt(Math.Pow(xForce[particlesCursor], 2.0) + Math.Pow(yForce[particlesCursor], 2.0)) < 4.2)
                {
                    xVelocity[particlesCursor] += xForce[particlesCursor] / 10;
                    yVelocity[particlesCursor] += yForce[particlesCursor] / 10;
                }
                else
                {
                    xVelocity[particlesCursor] += xForce[particlesCursor] / 11;
                    yVelocity[particlesCursor] += yForce[particlesCursor] / 11;
                }

                // velocity affects position
                xPos[particlesCursor] += xVelocity[particlesCursor];
                yPos[particlesCursor] += yVelocity[particlesCursor];
            }


            // given the position of the particle, determine the screen buffer
            // position that it's going to be in.
            x = (int)xPos[particlesCursor];
            // y scale correction, since each cell of the input map has
            // "2" rows in the particle space.
            y = (int)(yPos[particlesCursor] / 2);
            screenBufferIndex = x + CONSOLE_WIDTH * y;


            // if the particle is on screen, update
            // four buffer cells around it
            // in a manner of a "gradient",
            // the representation of 1 particle will be like this:
            //
            //      8 4
            //      2 1
            //
            // which after the lookup that puts chars on the
            // screen will look like:
            //
            //      ,.
            //      `'
            //
            // With this mechanism, each particle creates
            // a gradient over a small area (four screen locations).
            // As the gradients of several particles "mix",
            // (because the bits are flipped
            // independently),
            // a character will be chosen such that
            // it gives an idea of what's going on under it.
            // You can see how corners can only have values of 8,4,2,1
            // which will have suitably "pointy" characters.
            // A "long vertical edge" (imagine two particles above another)
            // would be like this:
            //
            //      8  4
            //      10 5
            //      2  1
            //
            // and hence 5 and 10 are both vertical bars.
            // Same for horizontal edges (two particles aside each other)
            //
            //      8  12 4
            //      2  3  1
            //
            // and hence 3 and 12 are both horizontal dashes.
            // ... and so on for the other combinations such as
            // particles placed diagonally, where the diagonal bars
            // are used, and places where four particles are present,
            // in which case the highest number is reached, 15, which
            // maps into the blackest character of the sequence, '#'

            if (y >= 0 && y < CONSOLE_HEIGHT - 1 && x >= 0 && x < CONSOLE_WIDTH - 1)
            {
                screenBuffer[screenBufferIndex] |= 8; // set 4th bit to 1
                screenBuffer[screenBufferIndex + 1] |= 4; // set 3rd bit to 1
                                                          // now the cell in row below
                screenBuffer[screenBufferIndex + CONSOLE_WIDTH] |= 2; // set 2nd bit to 1
                screenBuffer[screenBufferIndex + CONSOLE_WIDTH + 1] |= 1; // set 1st bit to 1
            }

        }

        // Update the screen buffer
        for (screenBufferIndex = 0; screenBufferIndex < CharCount; screenBufferIndex++)
        {
            //if (screenBufferIndex % CONSOLE_WIDTH == CONSOLE_WIDTH - 1)
            //    screenBuffer[screenBufferIndex] = '\n';
            //else
            if (screenBufferIndex % CONSOLE_WIDTH != CONSOLE_WIDTH - 1)
                // the string below contains 16 characters, which is for all
                // the possible combinations of values in the screenbuffer since
                // it can be subject to flipping of the first 4 bits
                screenBuffer[screenBufferIndex] = (byte)" '`-.|//,\\|\\_\\/#"[screenBuffer[screenBufferIndex]];
            // ---------------------- the mappings --------------
            // 0  maps into space
            // 1  maps into '    2  maps into `    3  maps into -
            // 4  maps into .    5  maps into |    6  maps into /
            // 7  maps into /    8  maps into ,    9  maps into \
            // 10 maps into |    11 maps into \    12 maps into _
            // 13 maps into \    14 maps into /    15 maps into #
        }

        base.Update(delta);
    }

    public override void Render(TimeSpan delta)
    {
        for (int i = 0; i < screenBuffer.Length; i++)
        {
            Surface[i].Glyph = screenBuffer[i];
        }
        base.Render(delta);
    }
}