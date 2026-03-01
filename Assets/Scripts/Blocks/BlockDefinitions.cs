// ── BlockDefinitions.cs ───────────────────────────────────────────────────────
// Place in: Assets/Scripts/Blocks/BlockDefinitions.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Complete catalogue of all block shapes with every relevant rotation baked in.
/// Each shape is stored in ALL of its distinct orientations as separate entries
/// so no runtime rotation is needed (matching 1010!/Block Puzzle gameplay).
///
/// Shape coordinate system: col=x (right), row=y (up), origin at min corner.
/// </summary>
public static class BlockDefinitions
{
    // ── Colour palette ────────────────────────────────────────────────────────
    private static readonly Color[] Palette =
    {
        new Color(0.55f, 0.28f, 0.90f),   // purple
        new Color(0.15f, 0.76f, 0.32f),   // green
        new Color(0.96f, 0.45f, 0.08f),   // orange
        new Color(0.10f, 0.72f, 0.88f),   // cyan
        new Color(0.93f, 0.73f, 0.05f),   // yellow
        new Color(0.95f, 0.22f, 0.28f),   // red
        new Color(0.20f, 0.50f, 0.95f),   // royal blue
        new Color(0.95f, 0.30f, 0.70f),   // pink
    };

    // =========================================================================
    // ── Shape library ─────────────────────────────────────────────────────────
    // =========================================================================
    //  Visual diagrams use:   X = filled cell,  . = empty
    //  Columns go →  /  Rows go ↑  (row 0 = bottom)
    // =========================================================================

    private static readonly Vector2Int[][] AllShapes =
    {
        //──────────────────────────────────────────────────────────────────────
        // 1-CELL
        //──────────────────────────────────────────────────────────────────────
        /* 00 */  S(V(0,0)),                                      // X

        //──────────────────────────────────────────────────────────────────────
        // 2-CELL
        //──────────────────────────────────────────────────────────────────────
        /* 01 */  S(V(0,0),V(0,1)),                               // vertical domino
        /* 02 */  S(V(0,0),V(1,0)),                               // horizontal domino

        //──────────────────────────────────────────────────────────────────────
        // 3-CELL LINES
        //──────────────────────────────────────────────────────────────────────
        /* 03 */  S(V(0,0),V(0,1),V(0,2)),                        // │││ vertical
        /* 04 */  S(V(0,0),V(1,0),V(2,0)),                        // ─── horizontal

        //──────────────────────────────────────────────────────────────────────
        // 4-CELL LINES
        //──────────────────────────────────────────────────────────────────────
        /* 05 */  S(V(0,0),V(0,1),V(0,2),V(0,3)),
        /* 06 */  S(V(0,0),V(1,0),V(2,0),V(3,0)),

        //──────────────────────────────────────────────────────────────────────
        // 5-CELL LINES
        //──────────────────────────────────────────────────────────────────────
        /* 07 */  S(V(0,0),V(0,1),V(0,2),V(0,3),V(0,4)),
        /* 08 */  S(V(0,0),V(1,0),V(2,0),V(3,0),V(4,0)),

        //──────────────────────────────────────────────────────────────────────
        // 2×2 SQUARE
        //──────────────────────────────────────────────────────────────────────
        /* 09 */  S(V(0,0),V(1,0),V(0,1),V(1,1)),                 // XX / XX

        //──────────────────────────────────────────────────────────────────────
        // 3×3 SQUARE
        //──────────────────────────────────────────────────────────────────────
        /* 10 */  S(V(0,0),V(1,0),V(2,0),
                    V(0,1),V(1,1),V(2,1),
                    V(0,2),V(1,2),V(2,2)),

        //──────────────────────────────────────────────────────────────────────
        // SMALL L-SHAPES  (3 cells, 4 rotations each)
        //
        //  rot0: XX    rot1: X.    rot2: .X    rot3: XX
        //        X.          XX          .X          .X
        //──────────────────────────────────────────────────────────────────────
        /* 11 */  S(V(0,0),V(0,1),V(1,1)),   // rot0  ┘
        /* 12 */  S(V(0,0),V(1,0),V(0,1)),   // rot1  └
        /* 13 */  S(V(1,0),V(0,1),V(1,1)),   // rot2  ┌
        /* 14 */  S(V(0,0),V(1,0),V(1,1)),   // rot3  ┐

        //──────────────────────────────────────────────────────────────────────
        // LARGE L-SHAPES  (4 cells, 4 rotations each)
        //
        //  arm of 3 + single corner cell
        //──────────────────────────────────────────────────────────────────────
        // ── Foot goes right (J-tetromino variants) ────────────────────────
        /* 15 */  S(V(0,0),V(0,1),V(0,2),V(1,0)),   // │+─
        /* 16 */  S(V(0,0),V(1,0),V(1,1),V(1,2)),   // ─+│ (rotated)
        /* 17 */  S(V(0,2),V(1,0),V(1,1),V(1,2)),   // ─+│ top
        /* 18 */  S(V(0,0),V(0,1),V(0,2),V(1,2)),   // │+─ top

        // ── Foot goes left (L-tetromino variants) ─────────────────────────
        /* 19 */  S(V(0,0),V(1,0),V(2,0),V(2,1)),
        /* 20 */  S(V(0,0),V(0,1),V(1,0),V(2,0)),
        /* 21 */  S(V(0,1),V(1,1),V(2,0),V(2,1)),
        /* 22 */  S(V(0,0),V(0,1),V(1,1),V(2,1)),

        //──────────────────────────────────────────────────────────────────────
        // 3×3 CORNER L-SHAPES  (5 cells, 4 rotations)
        //  Two arms of length 3 meeting at a corner — like a big bracket
        //
        //  BL:  X..    BR:  ..X    TL:  XXX    TR:  XXX
        //       X..         ..X         X..         ..X
        //       XXX         XXX         X..         ..X
        //──────────────────────────────────────────────────────────────────────
        /* 23 */  S(V(0,0),V(1,0),V(2,0),V(0,1),V(0,2)),   // BL corner
        /* 24 */  S(V(0,0),V(1,0),V(2,0),V(2,1),V(2,2)),   // BR corner
        /* 25 */  S(V(0,0),V(0,1),V(0,2),V(1,2),V(2,2)),   // TL corner
        /* 26 */  S(V(0,2),V(1,2),V(2,0),V(2,1),V(2,2)),   // TR corner

        //──────────────────────────────────────────────────────────────────────
        // T-SHAPES  (4 cells, 4 rotations)
        //──────────────────────────────────────────────────────────────────────
        /* 27 */  S(V(0,0),V(1,0),V(2,0),V(1,1)),   // ⊥  T pointing up
        /* 28 */  S(V(1,0),V(0,1),V(1,1),V(2,1)),   // ┬  T pointing down
        /* 29 */  S(V(0,0),V(0,1),V(0,2),V(1,1)),   // ├  T pointing right
        /* 30 */  S(V(0,1),V(1,0),V(1,1),V(1,2)),   // ┤  T pointing left

        //──────────────────────────────────────────────────────────────────────
        // S / Z SHAPES  (4 cells, 2 distinct orientations each)
        //──────────────────────────────────────────────────────────────────────
        /* 31 */  S(V(0,0),V(1,0),V(1,1),V(2,1)),   // S horizontal
        /* 32 */  S(V(0,1),V(1,0),V(1,1),V(2,0)),  // Z horizontal
        /* 33 */  S(V(0,1),V(0,2),V(1,0),V(1,1)),   // S vertical
        /* 34 */  S(V(0,0),V(0,1),V(1,1),V(1,2)),   // Z vertical

        //──────────────────────────────────────────────────────────────────────
        // 3-STEP DIAGONAL STAIRCASE  (5 cells)
        //  Single-cell-wide steps climbing diagonally
        //
        //  Ascending (↗):   ..X     Descending (↘):  X..
        //                   .XX                      XX.
        //                   XX.                      .XX
        //──────────────────────────────────────────────────────────────────────
        /* 35 */  S(V(0,0),V(1,0),V(1,1),V(2,1),V(2,2)),   // ascending  ↗
        /* 36 */  S(V(0,2),V(0,1),V(1,1),V(1,0),V(2,0)),   // descending ↘

        //──────────────────────────────────────────────────────────────────────
        // 4-STEP DIAGONAL STAIRCASE  (7 cells — only for late game feel)
        //
        //  Ascending:  ...X
        //              ..XX
        //              .XX.
        //              XX..
        //──────────────────────────────────────────────────────────────────────
        /* 37 */  S(V(0,0),V(1,0),V(1,1),V(2,1),V(2,2),V(3,2),V(3,3)),  // ascending

        //──────────────────────────────────────────────────────────────────────
        // PLUS / CROSS  (5 cells)
        //
        //   .X.
        //   XXX
        //   .X.
        //──────────────────────────────────────────────────────────────────────
        /* 38 */  S(V(1,0),V(0,1),V(1,1),V(2,1),V(1,2)),

        //──────────────────────────────────────────────────────────────────────
        // SMALL CROSS / PLUS  (3 cells — tiny T variants already covered)
        // U-SHAPES  (5 cells, 4 rotations)
        //
        //  Open top:   X.X    Open bot:   XXX    Open left:  XX    Open right: XX
        //              X.X               X.X                X.                .X
        //              XXX               X.X                XX                .X
        //                                                                     XX
        //──────────────────────────────────────────────────────────────────────
        /* 39 */  S(V(0,0),V(1,0),V(2,0),V(0,1),V(2,1)),            // U open top
        /* 40 */  S(V(0,0),V(2,0),V(0,1),V(1,1),V(2,1)),            // U open bot
        /* 41 */  S(V(0,0),V(0,1),V(0,2),V(1,0),V(1,2)),            // U open left (C shape)
        /* 42 */  S(V(0,0),V(1,0),V(1,1),V(1,2),V(0,2)),            // U open right (reverse C)

        //──────────────────────────────────────────────────────────────────────
        // 2×3  /  3×2 RECTANGLES  (6 cells)
        //──────────────────────────────────────────────────────────────────────
        /* 43 */  S(V(0,0),V(1,0),
                    V(0,1),V(1,1),
                    V(0,2),V(1,2)),   // 2 wide × 3 tall

        /* 44 */  S(V(0,0),V(1,0),V(2,0),
                    V(0,1),V(1,1),V(2,1)),   // 3 wide × 2 tall

        //──────────────────────────────────────────────────────────────────────
        // SNAKE / BENT SHAPES  (5 cells)
        //  Like a line that bends once at the midpoint
        //
        //   Bend right:  X..    Bend left:  ..X    Bend up:  XXX.   Bend down: .XXX
        //                XX.               .XX               ...X              X...
        //                .XX               XX.               ...X              X...
        //──────────────────────────────────────────────────────────────────────
        /* 45 */  S(V(0,2),V(0,1),V(1,1),V(2,1),V(2,0)),   // S-bend 5
        /* 46 */  S(V(0,0),V(0,1),V(1,1),V(2,1),V(2,2)),   // Z-bend 5
        /* 47 */  S(V(0,0),V(1,0),V(1,1),V(1,2),V(2,2)),   // hook right
        /* 48 */  S(V(0,2),V(1,0),V(1,1),V(1,2),V(2,0)),   // hook left

        //──────────────────────────────────────────────────────────────────────
        // 5-CELL EXTENDED L-SHAPES  (arm of 4 + perpendicular foot)
        //──────────────────────────────────────────────────────────────────────
        /* 49 */  S(V(0,0),V(1,0),V(2,0),V(3,0),V(0,1)),   // ───┘
        /* 50 */  S(V(0,0),V(1,0),V(2,0),V(3,0),V(3,1)),   // └───
        /* 51 */  S(V(0,0),V(0,1),V(0,2),V(0,3),V(1,0)),   // vertical + bottom foot right
        /* 52 */  S(V(0,0),V(0,1),V(0,2),V(0,3),V(1,3)),   // vertical + top foot right

        //──────────────────────────────────────────────────────────────────────
        // 5-CELL T-SHAPES  (arm of 4 + centre perpendicular)
        //──────────────────────────────────────────────────────────────────────
        /* 53 */  S(V(0,0),V(1,0),V(2,0),V(3,0),V(1,1)),   // ─┬──
        /* 54 */  S(V(0,0),V(1,0),V(2,0),V(3,0),V(2,1)),   // ──┬─
    };

    // =========================================================================
    // ── Weight pools (shape indices + probability) ────────────────────────────
    // =========================================================================
    //  Tiny  (≤2 cells) — appear often; easy to squeeze into gaps
    //  Small (3–4 cells) — standard frequency
    //  Medium(4–5 cells) — somewhat common
    //  Large (5–7 cells) — rarer; punishing on tight grids

    private static readonly int[] TinyPool   = { 0, 1, 2 };
    private static readonly int[] SmallPool  = { 3,4,9,11,12,13,14,31,32,33,34 };
    private static readonly int[] MediumPool = { 5,6,15,16,17,18,19,20,21,22,
                                                  27,28,29,30,35,36,38,39,40,41,42 };
    private static readonly int[] LargePool  = { 7,8,10,23,24,25,26,37,43,44,
                                                  45,46,47,48,49,50,51,52,53,54 };

    // =========================================================================
    // ── Public API ────────────────────────────────────────────────────────────
    // =========================================================================

    /// <summary>
    /// Returns a random BlockData, adapting the difficulty to the current grid state.
    ///
    /// When the grid is crowded (>55 % filled), the method:
    ///  1. Tries up to MAX_RETRIES times to find a shape that fits somewhere.
    ///  2. If all retries fail, returns a single-cell piece (guaranteed to fit
    ///     as long as any cell is empty).
    /// </summary>
    public static BlockData GetRandom(float occupancyRatio = 0f)
    {
        // Pick a pool based on how full the grid is
        int[] pool = ChoosePool(occupancyRatio);
        Color color = RandomColor();

        // On a tight grid: retry up to 12 times to find a piece that fits
        bool gridTight = occupancyRatio > 0.55f;
        int  maxTries  = gridTight ? 12 : 1;

        for (int attempt = 0; attempt < maxTries; attempt++)
        {
            var shape = AllShapes[pool[Random.Range(0, pool.Length)]];
            var data  = new BlockData(shape, color);

            if (!gridTight || GridManager.Instance == null ||
                GridManager.Instance.CanPlaceAnywhere(data))
                return data;
        }

        // Fallback — always fits if any cell is free
        return new BlockData(AllShapes[0], color);  // single cell
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    /// <summary>
    /// Selects a pool weighted toward smaller pieces as the grid fills up.
    ///
    ///  occupancy < 35 %  →  30 % tiny / 35 % small / 25 % medium / 10 % large
    ///  occupancy 35–55 % →  20 % tiny / 40 % small / 30 % medium / 10 % large
    ///  occupancy 55–70 % →  35 % tiny / 40 % small / 20 % medium /  5 % large
    ///  occupancy > 70 %  →  50 % tiny / 40 % small / 10 % medium /  0 % large
    /// </summary>
    private static int[] ChoosePool(float occ)
    {
        float roll = Random.value;

        if (occ < 0.35f)
        {
            if (roll < 0.30f) return TinyPool;
            if (roll < 0.65f) return SmallPool;
            if (roll < 0.90f) return MediumPool;
            return LargePool;
        }
        else if (occ < 0.55f)
        {
            if (roll < 0.20f) return TinyPool;
            if (roll < 0.60f) return SmallPool;
            if (roll < 0.90f) return MediumPool;
            return LargePool;
        }
        else if (occ < 0.70f)
        {
            if (roll < 0.35f) return TinyPool;
            if (roll < 0.75f) return SmallPool;
            if (roll < 0.95f) return MediumPool;
            return LargePool;
        }
        else  // > 70 % — grid very full
        {
            if (roll < 0.50f) return TinyPool;
            if (roll < 0.90f) return SmallPool;
            return MediumPool;   // large pieces never appear on a packed grid
        }
    }


    /// <summary>
    /// Returns the smallest shape that can still be placed on the current grid.
    /// Tries 1-cell, then 2-cell, then 3-cell shapes before giving up.
    /// </summary>
    public static BlockData GetGuaranteedFit()
    {
        Color color = RandomColor();
        var gm = GridManager.Instance;

        // Escalate through tiny→small pools until we find something that fits
        int[][] escalate = { TinyPool, SmallPool, MediumPool };
        foreach (var pool in escalate)
        {
            // Shuffle pool order so we don't always pick index 0
            var indices = new List<int>(pool);
            for (int i = indices.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int tmp = indices[i]; indices[i] = indices[j]; indices[j] = tmp;
            }
            foreach (int idx in indices)
            {
                var data = new BlockData(AllShapes[idx], color);
                if (gm == null || gm.CanPlaceAnywhere(data)) return data;
            }
        }

        // Absolute fallback: single cell always fits if any cell is free
        return new BlockData(AllShapes[0], color);
    }

    private static Color RandomColor() => Palette[Random.Range(0, Palette.Length)];

    // ── Shape construction helpers ────────────────────────────────────────────

    private static Vector2Int   V(int c, int r) => new Vector2Int(c, r);
    private static Vector2Int[] S(params Vector2Int[] cells) => cells;
}
