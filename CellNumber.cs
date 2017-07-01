using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CellNumber
{
    public int x;
    public int y;

    public CellNumber(int X, int Y)
    {
        x = X;
        y = Y;
    }

    public bool Equals(CellNumber cellNumber)
    {
        bool flag = false;
        if ((cellNumber.x == x) && (cellNumber.y == y))
        {
            flag = true;
        }
        return flag;
    }

    public static Vector3 GetCellPosition(CellNumber cellNumber)
    {
        float height = Mathf.Sqrt(1 - 0.5f * 0.5f);
        Vector3 position = new Vector3();
        position.x = cellNumber.x * 2 * height;
        if (cellNumber.y % 2 == 1)
            position.x += height;
        position.z = cellNumber.y * (1.5f);
        position.z *= -1;

        return position;
    }

    public List<CellNumber> GetAllNeighborCells()
    {
        List<CellNumber> list = new List<CellNumber>();

        list.Add(new CellNumber(x - 1, y));
        list.Add(new CellNumber(x - 1, y - 1));
        list.Add(new CellNumber(x, y - 1));
        list.Add(new CellNumber(x + 1, y - 1));
        list.Add(new CellNumber(x + 1, y));
        list.Add(new CellNumber(x + 1, y + 1));
        list.Add(new CellNumber(x, y + 1));
        list.Add(new CellNumber(x - 1, y + 1));

        if (y % 2 == 0)
        {
            list.RemoveAt(5);
            list.RemoveAt(3);
        }
        else
        {
            list.RemoveAt(7);
            list.RemoveAt(1);
        }
        return list;
    }

    public List<CellNumber> GetNeighborCells()
    {
        List<CellNumber> list = this.GetAllNeighborCells();

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (((list[i].x >= SteamcopterManager.steamcopterCellArrayAmount) || (list[i].x < 0)) ||
                ((list[i].y >= SteamcopterManager.steamcopterCellArrayAmount) || (list[i].y < 0)))
            {
                list.RemoveAt(i);
            }
        }

        return list;
    }

    public static Vector3 RotateVector(Vector3 Vector, float Angle)
    {
        Vector3 rez = new Vector3(0, Vector.y, 0);
        float k = 1;
        rez.x = Vector.x * Mathf.Cos(Angle) + k * Vector.z * Mathf.Sin(Angle);
        rez.z = -k * Vector.x * Mathf.Sin(Angle) + Vector.z * Mathf.Cos(Angle);
        return rez;
    }
}
