﻿// ReSharper disable RedundantExplicitArrayCreation

using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using SimpleTweaksPlugin;

namespace SecretTweaks; 

public static class Doom {

    private static double[] cam = { 7.7, 1.8 };
    private static double dir = -Math.PI / 2;
    private static double[][] enemies = {
        new double[] { 29, -27 },
        new double[] { 35, -19 },
        new double[] { 24, -22 },
    };
    
    private const double FloatingPointEqualTolerance = 0.0000001;
    
    private static Random random = new();
    
    public static double GetSliderValue(float t, float x, int i) {
        {
            var pressed = Service.KeyState;
            if (i == 0) {
            
                ImGui.Text($"{pressed[37]}, {pressed[39]}, {pressed[38]}, {pressed[40]}, {pressed[32]}");
            
            
                if (pressed[37]) dir -= 0.04;
                if (pressed[39]) dir += 0.04;
                if (pressed[38]) { 
                    cam[0] += 0.2*Math.Cos(dir);
                    cam[1] += 0.2 * Math.Sin(dir);
                }

                if (pressed[40]) {
                    cam[0] -= 0.2*Math.Cos(dir);
                    cam[1] -= 0.2 * Math.Sin(dir);
                }

                for (var eIndex = 0; eIndex < enemies.Length; eIndex++) {
                    var e = enemies[eIndex];
                    var d = Math.Sqrt(Math.Pow((e[0] - cam[0]), 2) + Math.Pow((e[1] - cam[1]), 2));
                    if (d < 10) {
                        var a2 = Math.Atan2(e[1] - cam[1], e[0] - cam[0]);
                        e[0] -= 0.03*Math.Cos(a2);
                        e[1] -= 0.03 * Math.Sin(a2);
                    }
                }
            }

            double[] intersect(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4) {
                if ((Math.Abs(x1 - x2) < FloatingPointEqualTolerance && Math.Abs(y1 - y2) < FloatingPointEqualTolerance) || (Math.Abs(x3 - x4) < FloatingPointEqualTolerance && Math.Abs(y3 - y4) < FloatingPointEqualTolerance)) return null;
                var d = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
                if (d == 0) return null;
                var ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / d;
                var ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / d;
                if (ua < 0 || ua > 1 || ub < 0 || ub > 1) return null;
                return new[] { x1 + ua * (x2 - x1), y1 + ua * (y2 - y1) };
            }
          
            double[] intersect2(double x1, double y1, double x2, double y2, double cx, double cy, double r) {
                var v1x = x2 - x1;
                var v1y = y2 - y1;
                var v2x = x1 - cx;
                var v2y = y1 - cy;
                var b = -2 * (v1x * v2x + v1y * v2y);
                var c = 2 * (v1x * v1x + v1y * v1y);
                var d = Math.Sqrt(b * b - 2 * c * (v2x * v2x + v2y * v2y - r * r));
                if (double.IsNaN(d)) return null;
                var u1 = (b - d) / c;
                if (u1 is <= 1 and >= 0) {
                    return new[] { x1 + v1x * u1, y1 + v1y * u1 };
                }
                return null;
            }

            var aa = Math.Atan((x-0.5)*1.5);
            var a = (dir + aa);
            var end = new double[] { cam[0] + Math.Cos(a) * 999, cam[1] + Math.Sin(a) * 999 };
          
            if ((i == 32 || i == 31) && pressed[32]) {

                for (var index = 0; index < enemies.Length; i++) {
                    var e = enemies[index];
                    var inter = intersect2(cam[0], cam[1], end[0], end[1], e[0], e[1], 0.3);
                    if (inter is { Length: > 0 }) {
                        // DIE!
                        var enemiesList = enemies.ToList();
                        enemiesList.RemoveAt(index);
                        enemies = enemiesList.ToArray();
                    }
                }
            
            }

            if (pressed[32]) {
                if (i == 29 || i == 34) {
                    return 0.05;
                }
                if (i == 31 || i == 32) {
                    return 0.2;
                }
            }

            var walls = new List<double[][]>() {
                new double[][] { new double[] { 0,0 },new double[] { 2.8,0 } }, 
                new double[][] { new double[] { 2.8,0 },new double[] { 5.6,2.1 } }, 
                new double[][] { new double[] { 5.6,2.1 },new double[] { 8.4,2.1 } }, 
                new double[][] { new double[] { 8.4,2.1 },new double[] { 9.1,2.1 } },
                new double[][] { new double[] { 9.1,2.1 },new double[] { 12.5,0 } },
                new double[][] { new double[] { 12.5,0 },new double[] { 14,0 } },
                new double[][] { new double[] { 14,0 },new double[] { 14,-14.3 } },
                new double[][] { new double[] { 14,-14.3 },new double[] { 14.9,-20.7 } },
                new double[][] { new double[] { 14.9,-20.7 },new double[] { 16.7,-21.5 } },
                new double[][] { new double[] { 16.7,-21.5 },new double[] { 20.8,-21.5 } },
                new double[][] { new double[] { 20.8,-21.5 },new double[] { 21.1,-15.8 } },
                new double[][] { new double[] { 21.1,-15.8 },new double[] { 38.7,-15.8 } },
                new double[][] { new double[] { 38.7,-15.8 },new double[] { 38.7,-32 } },
                new double[][] { new double[] { 38.7,-32 },new double[] { 21,-32 } },
                new double[][] { new double[] { 21,-32 },new double[] { 21,-24.2 } },
                new double[][] { new double[] { 21,-24.2 },new double[] { 16.8,-24 } },
                new double[][] { new double[] { 16.8,-24 },new double[] { 11.9,-22 } },
                new double[][] { new double[] { 11.9,-22 },new double[] { 11.1,-14.5 } },
                new double[][] { new double[] { 11.1,-14.5 },new double[] { 5.7,-14.4 } },
                new double[][] { new double[] { 5.7,-14.4 },new double[] { 2.9,-13.1 } },
                new double[][] { new double[] { 2.9,-13.1 },new double[] { 0,-13 } },
                new double[][] { new double[] { 0,-13 },new double[] { 0,-9.7 } },
                new double[][] { new double[] { 0,-9.7 },new double[] { -4.2,-8.7 } },
                new double[][] { new double[] { -4.2,-8.7 },new double[] { -5.3,-11.5 } },
                new double[][] { new double[] { -5.3,-11.5 },new double[] { -12.3,-11.5 } },
                new double[][] { new double[] { -12.3,-11.5 },new double[] { -12.3,-2.1 } },
                new double[][] { new double[] { -12.3,-2.1 },new double[] { -5.4,-2.1 } },
                new double[][] { new double[] { -5.4,-2.1 },new double[] { -4.2,-4.9 } },
                new double[][] { new double[] { -4.2,-4.9 },new double[] { 0,-4 } },
                new double[][] { new double[] { 0,-4 },new double[] { 0,0 } }
            };
          
            void AddColumn(double cx, double cy) {
                var cr = 0.3;
                walls.Add(new double[][] { new double[] { cx-cr,cy-cr },new double[] {cx-cr,cy+cr}});
                walls.Add(new double[][] { new double[] { cx-cr,cy+cr},new double[] {cx+cr,cy+cr}});
                walls.Add(new double[][] { new double[] { cx+cr,cy+cr},new double[] {cx+cr,cy-cr}});
                walls.Add(new double[][] { new double[] { cx+cr,cy-cr},new double[] {cx-cr,cy-cr}});
            }

            AddColumn(4.6, -3.7);
            AddColumn(10.8, -3.7);
            AddColumn(10.8, -10);
            AddColumn(4.6, -10);

          
            var xs = walls.Select((w) => intersect(cam[0], cam[1], end[0], end[1], w[0][0], w[0][1], w[1][0], w[1][1])).Where((p) => p != null).Select((p) => Math.Sqrt(Math.Pow((p[0] - cam[0]), 2) + Math.Pow((p[1] - cam[1]), 2)));
            var nearestWall = xs.Any() ? xs.Min() : 9999;

            var xs2 = enemies.Select((e) => intersect2(cam[0], cam[1], end[0], end[1], e[0], e[1], 0.3)).Where((p) => p != null && p.Length > 0).Select((p) => Math.Sqrt(Math.Pow((p[0] - cam[0]), 2) + Math.Pow((p[1] - cam[1]), 2)));
          
            var nearestEnemy = xs2.Any() ? xs2.Min() : 9999;
            if (nearestWall < nearestEnemy) {
                return 0.5 + ((1 / (Math.Cos(aa) * nearestWall)) * 2 * ((i % 2) - 0.5));
            } else {
                return 0.5 + ((1 / (Math.Cos(aa) * nearestEnemy)) * 1 * ((i % 2) - 0.75)) - random.NextDouble() * 0.1 * (1 / nearestEnemy);
            }
        }
    }
}
