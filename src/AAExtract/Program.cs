// <copyright file="Program.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using AAExtract;
using ConsoleAppFramework;

var app = ConsoleApp.Create();
app.Add<Commands>();
app.Run(args);