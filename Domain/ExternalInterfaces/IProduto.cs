﻿using Entities.ExternalEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ExternalInterfaces
{
    public interface IProduto
    {
        List<Produto> List();
        Produto Create(Produto produto);

        Produto Update(Produto produto);

        Produto GetOne(int Codigo);

        Produto Delete(int Codigo);
    }
}
