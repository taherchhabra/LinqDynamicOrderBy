An Extension method on IQueryable for using OrderBy with dynamic column names.

Usage
context.Products.DynamicOrderBy("ProductName ASC,Category DESC");