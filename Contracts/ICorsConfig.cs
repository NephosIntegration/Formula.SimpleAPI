using System;

namespace Formula.SimpleAPI
{
    public interface ICorsConfig
    {
        String[] GetOrigins();
    }
}