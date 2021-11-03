using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SadConsole;
using SadConsole.Components;
using SadRogue.Primitives;

namespace SadExperimentsV9
{
    class Donut : TestConsole
    {
        public Donut() : base()
        {
            AddCentered(new Donut3D());
        }
    }

    // This is my C# implementation of the ASCII donut by Andy Sloane https://www.a1k0n.net/2011/07/20/donut-math.html
    class Donut3D : ScreenSurface
    {
        const double theta_spacing = 0.07f;
        const double phi_spacing = 0.02f;

        const double R1 = 1;
        const double R2 = 2;
        const double K2 = 5;

        readonly char[,] _output;
        readonly double[,] _zbuffer;
        readonly double _k1;
        readonly Cursor Cursor;

        double _a = 0.1, _b = 0.1;

        public Donut3D() : base(Program.Height, Program.Height)
        {
            // Calculate K1 based on screen size: the maximum x-distance occurs
            // roughly at the edge of the torus, which is at x=R1+R2, z=0.  we
            // want that to be displaced 3/8ths of the width of the screen, which
            // is 3/4th of the way from the center to the side of the screen.
            // Surface.Width*3/8 = K1*(R1+R2)/(K2+0)
            // Surface.Width*K2*3/(8*(R1+R2)) = K1
            _k1 = Surface.Width * K2 * 3 / (8 * (R1 + R2));

            _output = new char[Surface.Width, Surface.Height];
            _zbuffer = new double[Surface.Width, Surface.Height];

            Cursor = new Cursor()
            {
                IsEnabled = true,
                IsVisible = false,
                ApplyCursorEffect = true,
                PrintOnlyCharacterData = false
            };

            SadComponents.Add(Cursor);
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            _a += 0.07;
            _b += 0.03;

            Render_Frame(_a, _b);
        }

        void Render_Frame(double A, double B)
        {
            Array.Clear(_output, 0, _output.Length);
            Array.Clear(_zbuffer, 0, _zbuffer.Length);

            // precompute sines and cosines of A and B
            double cosA = Math.Cos(A), sinA = Math.Sin(A);
            double cosB = Math.Cos(B), sinB = Math.Sin(B);

            // theta goes around the cross-sectional circle of a torus
            for (double theta = 0; theta < 2 * Math.PI; theta += theta_spacing)
            {
                // precompute sines and cosines of theta
                double costheta = Math.Cos(theta), sintheta = Math.Sin(theta);

                // phi goes around the center of revolution of a torus
                for (double phi = 0; phi < 2 * Math.PI; phi += phi_spacing)
                {
                    // precompute sines and cosines of phi
                    double cosphi = Math.Cos(phi), sinphi = Math.Sin(phi);

                    // the x,y coordinate of the circle, before revolving (factored
                    // out of the above equations)
                    double circlex = R2 + R1 * costheta;
                    double circley = R1 * sintheta;

                    // final 3D (x,y,z) coordinate after rotations, directly from
                    // our math above
                    double x = circlex * (cosB * cosphi + sinA * sinB * sinphi)
                      - circley * cosA * sinB;
                    double y = circlex * (sinB * cosphi - sinA * cosB * sinphi)
                      + circley * cosA * cosB;
                    double z = K2 + cosA * circlex * sinphi + circley * sinA;
                    double ooz = 1 / z;  // "one over z"

                    // x and y projection.  note that y is negated here, because y
                    // goes up in 3D space but down on 2D displays.
                    int xp = (int)(Surface.Width / 2 + _k1 * ooz * x);
                    int yp = (int)(Surface.Height / 2 - _k1 * ooz * y);

                    // calculate luminance.  ugly, but correct.
                    double L = cosphi * costheta * sinB - cosA * costheta * sinphi -
                      sinA * sintheta + cosB * (cosA * sintheta - costheta * sinA * sinphi);
                    // L ranges from -sqrt(2) to +sqrt(2).  If it's < 0, the surface
                    // is pointing away from us, so we won't bother trying to plot it.
                    if (L > 0)
                    {
                        // test against the z-buffer.  larger 1/z means the pixel is
                        // closer to the viewer than what's already plotted.
                        if (ooz > _zbuffer[xp, yp])
                        {
                            _zbuffer[xp, yp] = ooz;
                            int luminance_index = Convert.ToInt32(L * 8);
                            // luminance_index is now in the range 0..11 (8*sqrt(2) = 11.3)
                            // now we lookup the character corresponding to the
                            // luminance and plot it in our output:
                            _output[xp, yp] = ".,-~:;=!*#$@"[luminance_index];
                        }
                    }
                }
            }

            // now, dump output[] to the screen.
            // bring cursor to "home" location, in just about any currently-used
            // terminal emulation mode
            Cursor.Position = (0, 0);
            for (int j = 0; j < Surface.Height; j++)
            {
                for (int i = 0; i < Surface.Width; i++)
                {
                    Cursor.Print(_output[i, j].ToString());
                }
            }

        }
    }
}
