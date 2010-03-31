libMatt: Matt's General-Purpose Toolbox

This class library is a collection of helpful classes and utilities that I find myself frequently using in various projects. All code here is under the MIT license (see LICENSE), with the exception of the BCrypt library by Damien Miller and Derek Slager, which is licensed under similarly permissive terms (see Security/BCrypt.cs).

The libMatt.Data namespace includes data provider classes for SQL Server and SQL Server Compact Edition. The latter relies on the System.Data.SqlServerCe.dll, included in the dependencies folder. (This originated when I decided to use a standalone SQL Server CE database for all the data-related tests.) If you have no need of this and wish to distribute your application without SqlServerCe, simply open the libMatt project properties, and under the "Build" tab, remove the conditional compilation symbol "USE_SQLSERVERCE". You may then remove the reference to SqlServerCe.dll and rebuild.

Note: The "dependencies" folder contains referenced binaries from other sources which may be licensed under other terms. See LICENSE for details.

-- Matt