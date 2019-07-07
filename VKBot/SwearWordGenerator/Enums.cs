using System;
using System.Collections.Generic;
using System.Text;

namespace SwearWordGenerator
{
    public enum Type
    {
        Noun, //существииетльное
        Adjective, //прилагательное
        Verb, //глагол
    }

    public enum Sex
    {
        M, //мужской
        F, //женский
        N, //средний или отсуствует
    }

    public enum Case
    {
        I, //именительный
        R, //родительный
        D, //дательный
        V, //винительный
        T, //творительный
        P, //предложный
    }

}
