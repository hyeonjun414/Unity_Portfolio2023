using System.Collections.Generic;

public class SeedRandom
{
    public uint m_w;
    public uint m_z;

    public double Value => GetUniform();

    public SeedRandom()
    {
        m_w = 521288629;
        m_z = 362436069;
    }

    public SeedRandom(uint v)
    {
        m_w = 521288629;
        m_z = 362436069;
        if (v != 0) m_z = v;
    }
    public void SetSeed(uint u, uint v)
    {
        if (u != 0) m_w = u;
        if (v != 0) m_z = v;
    }

    public void SetSeed(uint u)
    {
        m_z = u;
        m_w = GetUint();
    }

    public void SetSeedFromSystemTime()
    {
        System.DateTime dt = System.DateTime.Now;
        long x = dt.ToFileTime();
        SetSeed((uint) (x >> 16), (uint) (x % 4294967296));
    }

    public double GetUniform()
    {
        uint u = GetUint();
        return (u + 1.0) * 2.328306435454494e-10;
    }

    public int GetZeroToMann()
    {
        var u = GetUint();
        var zeroToMann = u / (uint.MaxValue / 10000);

        return (int) zeroToMann;
    }

    public int GetUniform(int max)
    {
        return (int)(GetUniform() * max);
    }
    public int GetUniformRange(int min, int max)
    {
        return min + (int) (GetUniform() * (max - min));
    }
    public int Range(int min, int max)
    {
        if(min == max)
            return min;
        
        var u = GetUint();
        var zeroToMann = u / (uint.MaxValue / (max-min));
        return (int) (min + zeroToMann);
    }

    public float Range(float min, float max)
    {
        var u = GetUint();
        var zeroToMann = u / (uint.MaxValue / (max - min));
        return min + zeroToMann;
    }

    public List<int> RangeMany(int min, int max, int cnt)
    {
        var ret = new List<int>();

        for (int i = 0; i < cnt; i++)
        {
            var value = Range(min, max-i);
            while (ret.Contains(value))
            {
                value++;
            }
            ret.Add(value);
        }
        
        return ret;
    }

    internal uint GetUint()
    {
        m_z = 36969 * (m_z & 65535) + (m_z >> 16);
        m_w = 18000 * (m_w & 65535) + (m_w >> 16);
        return (m_z << 16) + m_w;
    }
}