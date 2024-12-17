﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable BL0005 // Set parameter outside component

using System.Globalization;
using System.Reflection;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using Bunit.Rendering;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.UnitTests.TestComponents.DataGrid;
using MudBlazor.Utilities.Clone;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    public record TestModel1(string Name, int? Age);
    public record TestModel2(string Name, int? Age, DateTime? Date);
    public record TestModel3(string Name, int? Age, Severity? Status);
    public record TestModel4(string Name, int? Age, bool? Hired);

    [TestFixture]
    public class DataGridTests : BunitTest
    {
        [Test]
        [SetCulture("")]
        [SetUICulture("")]
        public void DataGridPropertyNullCheck()
        {
            var comp = Context.RenderComponent<DataGridPropertyColumnNullCheckTest>();
            var cells = comp.FindAll("td").ToArray();

            // First Row
            cells[0].TextContent.Should().Be("01/01/0001 00:00:00");
            cells[1].TextContent.Should().BeEmpty();
            cells[2].TextContent.Should().Be("01/01/0001 00:00:00");
            cells[3].TextContent.Should().BeEmpty();
            cells[4].TextContent.Should().BeEmpty();
            cells[5].TextContent.Should().BeEmpty();

            // Second Row
            cells[6].TextContent.Should().Be("01/01/0001 00:00:00");
            cells[7].TextContent.Should().Be("01/01/0001 00:00:00 +00:00");
            cells[8].TextContent.Should().Be("01/01/0001 00:00:00");
            cells[9].TextContent.Should().Be("01/01/0001 00:00:00");
            cells[10].TextContent.Should().Be("some text");
            cells[11].TextContent.Should().BeEmpty();
        }

        [Test]
        public async Task DataGridSortableTest()
        {
            var comp = Context.RenderComponent<DataGridSortableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSortableTest.Item>>();

            // Count the number of rows including header.
            var rows = dataGrid.FindAll("tr");
            rows.Count.Should().Be(9, because: "1 header row + 7 data rows + 1 footer row");

            var cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(21, because: "We have 7 data rows with three columns");

            // Check the values of rows without sorting
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted ascending by Name.
            cells[0].TextContent.Should().Be("A"); cells[1].TextContent.Should().Be("73"); cells[2].TextContent.Should().Be("7");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("11"); cells[5].TextContent.Should().Be("4444");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("99"); cells[8].TextContent.Should().Be("66");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("C"); cells[13].TextContent.Should().Be("33"); cells[14].TextContent.Should().Be("33333");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Descending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted descending by Name.
            cells[0].TextContent.Should().Be("C"); cells[1].TextContent.Should().Be("33"); cells[2].TextContent.Should().Be("33333");
            cells[3].TextContent.Should().Be("C"); cells[4].TextContent.Should().Be("44"); cells[5].TextContent.Should().Be("1111111");
            cells[6].TextContent.Should().Be("C"); cells[7].TextContent.Should().Be("55"); cells[8].TextContent.Should().Be("222222");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("73"); cells[14].TextContent.Should().Be("7");
            cells[15].TextContent.Should().Be("A"); cells[16].TextContent.Should().Be("11"); cells[17].TextContent.Should().Be("4444");
            cells[18].TextContent.Should().Be("A"); cells[19].TextContent.Should().Be("99"); cells[20].TextContent.Should().Be("66");

            await comp.InvokeAsync(() => dataGrid.Instance.RemoveSortAsync("Name"));
            cells = dataGrid.FindAll("td");

            // Back to original order without sorting
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            var column = dataGrid.FindComponent<Column<DataGridSortableTest.Item>>();
            await comp.InvokeAsync(() => column.Instance.SortBy = x => { return x.Name; });
            ////await comp.InvokeAsync(() => column.Instance.CompileSortBy());

            // Check the values of rows - should not be sorted and should be in the original order.
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            // sort through the sort icon
            dataGrid.Find(".column-options button").Click();
            cells = dataGrid.FindAll("td");
            // Check the values of rows - should be sorted ascending by Name.
            cells[0].TextContent.Should().Be("A"); cells[1].TextContent.Should().Be("73"); cells[2].TextContent.Should().Be("7");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("11"); cells[5].TextContent.Should().Be("4444");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("99"); cells[8].TextContent.Should().Be("66");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("C"); cells[13].TextContent.Should().Be("33"); cells[14].TextContent.Should().Be("33333");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            // test other sort methods
            var headerCell = dataGrid.FindComponent<HeaderCell<DataGridSortableTest.Item>>();
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));
            //await comp.InvokeAsync(() => headerCell.Instance.GetDataType());
            await comp.InvokeAsync(() => headerCell.Instance.RemoveSortAsync());
            await comp.InvokeAsync(() => headerCell.Instance.AddFilter());
            await comp.InvokeAsync(() => headerCell.Instance.OpenFilters());

            await comp.InvokeAsync(() => dataGrid.Instance.SortMode = SortMode.None);
            dataGrid.Render();
            dataGrid.Instance.DropContainerHasChanged();
            // Since Sortable is now false, the click handler (and element holding it) should no longer exist.
            dataGrid.FindAll(".column-header .sortable-column-header").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridSortableVirtualizeServerDataTest()
        {
            var comp = Context.RenderComponent<DataGridSortableVirtualizeServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSortableVirtualizeServerDataTest.Item>>();

            // Count the number of rows including header.
            var rows = dataGrid.FindAll("tr");
            rows.Count.Should().Be(9, because: "1 header row + 7 data rows + 1 footer row");

            var cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(21, because: "We have 7 data rows with three columns");

            // Check the values of rows without sorting
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted ascending by Name.
            cells[0].TextContent.Should().Be("A"); cells[1].TextContent.Should().Be("73"); cells[2].TextContent.Should().Be("7");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("11"); cells[5].TextContent.Should().Be("4444");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("99"); cells[8].TextContent.Should().Be("66");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("C"); cells[13].TextContent.Should().Be("33"); cells[14].TextContent.Should().Be("33333");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Descending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted descending by Name.
            cells[0].TextContent.Should().Be("C"); cells[1].TextContent.Should().Be("33"); cells[2].TextContent.Should().Be("33333");
            cells[3].TextContent.Should().Be("C"); cells[4].TextContent.Should().Be("44"); cells[5].TextContent.Should().Be("1111111");
            cells[6].TextContent.Should().Be("C"); cells[7].TextContent.Should().Be("55"); cells[8].TextContent.Should().Be("222222");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("73"); cells[14].TextContent.Should().Be("7");
            cells[15].TextContent.Should().Be("A"); cells[16].TextContent.Should().Be("11"); cells[17].TextContent.Should().Be("4444");
            cells[18].TextContent.Should().Be("A"); cells[19].TextContent.Should().Be("99"); cells[20].TextContent.Should().Be("66");

            await comp.InvokeAsync(() => dataGrid.Instance.RemoveSortAsync("Name"));
            cells = dataGrid.FindAll("td");

            // Back to original order without sorting
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            var column = dataGrid.FindComponent<Column<DataGridSortableVirtualizeServerDataTest.Item>>();
            await comp.InvokeAsync(() => column.Instance.SortBy = x => { return x.Name; });

            // Check the values of rows - should not be sorted and should be in the original order.
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            // sort through the sort icon
            dataGrid.Find(".column-options button").Click();
            cells = dataGrid.FindAll("td");
            // Check the values of rows - should be sorted ascending by Name.
            cells[0].TextContent.Should().Be("A"); cells[1].TextContent.Should().Be("73"); cells[2].TextContent.Should().Be("7");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("11"); cells[5].TextContent.Should().Be("4444");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("99"); cells[8].TextContent.Should().Be("66");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("C"); cells[13].TextContent.Should().Be("33"); cells[14].TextContent.Should().Be("33333");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            // test other sort methods
            var headerCell = dataGrid.FindComponent<HeaderCell<DataGridSortableVirtualizeServerDataTest.Item>>();
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));
            //await comp.InvokeAsync(() => headerCell.Instance.GetDataType());
            await comp.InvokeAsync(() => headerCell.Instance.RemoveSortAsync());
            await comp.InvokeAsync(() => headerCell.Instance.AddFilter());
            await comp.InvokeAsync(() => headerCell.Instance.OpenFilters());

            await comp.InvokeAsync(() => dataGrid.Instance.SortMode = SortMode.None);
            dataGrid.Render();
            dataGrid.Instance.DropContainerHasChanged();
            // Since Sortable is now false, the click handler (and element holding it) should no longer exist.
            dataGrid.FindAll(".column-header .sortable-column-header").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridSortableHeaderRowTest()
        {
            var comp = Context.RenderComponent<DataGridSortableHeaderRowTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSortableHeaderRowTest.Item>>();

            // Count the number of rows including header.
            var rows = dataGrid.FindAll("tr");
            rows.Count.Should().Be(6, because: "2 header rows + 3 data rows + 1 footer row");

            var cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(9, because: "We have 3 data rows with three columns");

            // Check the values of rows without sorting
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            cells[0].TextContent.Should().Be("A"); cells[1].TextContent.Should().Be("73"); cells[2].TextContent.Should().Be("7");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("11"); cells[5].TextContent.Should().Be("4444");
            cells[6].TextContent.Should().Be("B"); cells[7].TextContent.Should().Be("42"); cells[8].TextContent.Should().Be("555");
        }

        [Test]
        public async Task DataGridSortableTemplateColumnTest()
        {
            var comp = Context.RenderComponent<DataGridSortableTemplateColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSortableTemplateColumnTest.Item>>();

            // Count the number of rows including header.
            var rows = dataGrid.FindAll("tr");
            rows.Count.Should().Be(9, because: "1 header row + 7 data rows + 1 footer row");

            var cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(7, because: "We have 7 data rows with one column");

            // Check the values of rows without sorting
            cells[0].TextContent.Should().Be("B");
            cells[1].TextContent.Should().Be("A");
            cells[2].TextContent.Should().Be("A");
            cells[3].TextContent.Should().Be("C");
            cells[4].TextContent.Should().Be("A");
            cells[5].TextContent.Should().Be("C");
            cells[6].TextContent.Should().Be("C");

            // property name is the hash code of the template column
            var templatePropertyName = dataGrid.Instance.RenderedColumns.FirstOrDefault().PropertyName;

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(templatePropertyName, SortDirection.Ascending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted ascending by Name.
            cells[0].TextContent.Should().Be("A");
            cells[1].TextContent.Should().Be("A");
            cells[2].TextContent.Should().Be("A");
            cells[3].TextContent.Should().Be("B");
            cells[4].TextContent.Should().Be("C");
            cells[5].TextContent.Should().Be("C");
            cells[6].TextContent.Should().Be("C");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync(templatePropertyName, SortDirection.Descending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted descending by Name.
            cells[0].TextContent.Should().Be("C");
            cells[1].TextContent.Should().Be("C");
            cells[2].TextContent.Should().Be("C");
            cells[3].TextContent.Should().Be("B");
            cells[4].TextContent.Should().Be("A");
            cells[5].TextContent.Should().Be("A");
            cells[6].TextContent.Should().Be("A");

            await comp.InvokeAsync(() => dataGrid.Instance.RemoveSortAsync(templatePropertyName));
            cells = dataGrid.FindAll("td");

            // Back to original order without sorting
            cells[0].TextContent.Should().Be("B");
            cells[1].TextContent.Should().Be("A");
            cells[2].TextContent.Should().Be("A");
            cells[3].TextContent.Should().Be("C");
            cells[4].TextContent.Should().Be("A");
            cells[5].TextContent.Should().Be("C");
            cells[6].TextContent.Should().Be("C");

            // sort through the sort icon
            dataGrid.Find(".column-options button").Click();
            cells = dataGrid.FindAll("td");
            // Check the values of rows - should be sorted ascending by Name.
            cells[0].TextContent.Should().Be("A");
            cells[1].TextContent.Should().Be("A");
            cells[2].TextContent.Should().Be("A");
            cells[3].TextContent.Should().Be("B");
            cells[4].TextContent.Should().Be("C");
            cells[5].TextContent.Should().Be("C");
            cells[6].TextContent.Should().Be("C");
        }

        [Test]
        public async Task DataGridFilterableVirtualizeServerDataTest()
        {
            var comp = Context.RenderComponent<DataGridFilterableVirtualizeServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterableVirtualizeServerDataTest.Item>>();

            // Count the number of rows including header.
            dataGrid.FindAll("tr").Count.Should().Be(6, because: "header row + four rows + footer row");

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("C");

            // Add a FilterDefinition to filter where the Name = "C".
            await comp.InvokeAsync(() =>
            {
                return dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFilterableVirtualizeServerDataTest.Item>
                {
                    Column = dataGrid.Instance.RenderedColumns.First(),
                    Operator = FilterOperator.String.Equal,
                    Value = "C"
                });
            });

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("C");

            dataGrid.Instance.Filterable = false;
        }

        [Test]
        public async Task DataGridFilterableTest()
        {
            var comp = Context.RenderComponent<DataGridFilterableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterableTest.Item>>();

            // Count the number of rows including header.
            dataGrid.FindAll("tr").Count.Should().Be(6); // header row + four rows + footer row

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("C");

            // Add a FilterDefinition to filter where the Name = "C".
            await comp.InvokeAsync(() =>
            {
                return dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFilterableTest.Item>
                {
                    Column = dataGrid.Instance.RenderedColumns.First(),
                    Operator = FilterOperator.String.Equal,
                    Value = "C"
                });
            });

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("C");

            dataGrid.Instance.Filterable = false;
        }

        [Test]
        public void DataGrid_SetParameters_ServerData_Items_Throw()
        {
            var serverDataFunc =
                new Func<GridState<TestModel1>, Task<GridData<TestModel1>>>((x) => throw new NotImplementedException());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Context.RenderComponent<MudDataGrid<TestModel1>>(
                    Parameter(nameof(MudDataGrid<TestModel1>.ServerData), serverDataFunc),
                    Parameter(nameof(MudDataGrid<TestModel1>.Items), Array.Empty<TestModel1>())
                )
            );
            exception.Message.Should().Be(
                """
                MudBlazor.MudDataGrid`1[MudBlazor.UnitTests.Components.TestModel1] can only accept one item source from its parameters. Do not supply both 'Items' and 'ServerData'.
                """
            );
        }

        [Test]
        public void DataGrid_SetParameters_ServerData_QuickFilter_Throw()
        {
            var serverDataFunc =
                new Func<GridState<TestModel1>, Task<GridData<TestModel1>>>((x) => throw new NotImplementedException());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Context.RenderComponent<MudDataGrid<TestModel1>>(
                    Parameter(nameof(MudDataGrid<TestModel1>.ServerData), serverDataFunc),
                    Parameter(nameof(MudDataGrid<TestModel1>.QuickFilter), (TestModel1 x) => true)
                )
            );
            exception.Message.Should().Be("Do not supply both 'ServerData' and 'QuickFilter'.");
        }

        [Test]
        public void DataGrid_SetParameters_VirtualizeServerData_QuickFilter_Throw()
        {
            var virtualizeServerDataFunc =
                new Func<GridStateVirtualize<TestModel1>, CancellationToken, Task<GridData<TestModel1>>>((x, c) => throw new NotImplementedException());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Context.RenderComponent<MudDataGrid<TestModel1>>(
                    Parameter(nameof(MudDataGrid<TestModel1>.VirtualizeServerData), virtualizeServerDataFunc),
                    Parameter(nameof(MudDataGrid<TestModel1>.QuickFilter), (TestModel1 x) => true)
                )
            );
            exception.Message.Should().Be("Do not supply both 'VirtualizeServerData' and 'QuickFilter'.");
        }

        [Test]
        public void DataGrid_SetParameters_ServerData_VirtualizeServerData_Throw()
        {
            var serverDataFunc =
                new Func<GridState<TestModel1>, Task<GridData<TestModel1>>>((x) => throw new NotImplementedException());
            var virtualizeServerDataFunc =
                new Func<GridStateVirtualize<TestModel1>, CancellationToken, Task<GridData<TestModel1>>>((x, c) => throw new NotImplementedException());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Context.RenderComponent<MudDataGrid<TestModel1>>(
                    Parameter(nameof(MudDataGrid<TestModel1>.ServerData), serverDataFunc),
                    Parameter(nameof(MudDataGrid<TestModel1>.VirtualizeServerData), virtualizeServerDataFunc)
                )
            );

            exception.Message.Should().Be(
                """
                MudBlazor.MudDataGrid`1[MudBlazor.UnitTests.Components.TestModel1] can only accept one item source from its parameters. Do not supply both 'VirtualizeServerData' and 'ServerData'.
                """
            );
        }

        [Test]
        public void DataGrid_SetParameters_Items_VirtualizeServerData_Throw()
        {
            var virtualizeServerDataFunc =
                new Func<GridStateVirtualize<TestModel1>, CancellationToken, Task<GridData<TestModel1>>>((x, c) => throw new NotImplementedException());
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Context.RenderComponent<MudDataGrid<TestModel1>>(
                    Parameter(nameof(MudDataGrid<TestModel1>.Items), Array.Empty<TestModel1>()),
                    Parameter(nameof(MudDataGrid<TestModel1>.VirtualizeServerData), virtualizeServerDataFunc)
                )
            );

            exception.Message.Should().Be(
                """
                MudBlazor.MudDataGrid`1[MudBlazor.UnitTests.Components.TestModel1] can only accept one item source from its parameters. Do not supply both 'Items' and 'VirtualizeServerData'.
                """
            );
        }

        [Test]
        public async Task DataGridFilterableServerDataTest()
        {
            var comp = Context.RenderComponent<DataGridFilterableServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterableServerDataTest.Item>>();

            // Count the number of rows including header.
            dataGrid.FindAll("tr").Count.Should().Be(6); // header row + four rows + footer row

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("B");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("A");
            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("C");

            // Add a FilterDefinition to filter where the Name = "C".
            await comp.InvokeAsync(() =>
            {
                return dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFilterableServerDataTest.Item>
                {
                    Column = dataGrid.Instance.RenderedColumns.FirstOrDefault(),
                    Operator = FilterOperator.String.Equal,
                    Value = "C"
                });
            });

            // Check the values of rows
            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("C");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("C");

            dataGrid.Instance.Filterable = false;
        }

        [Test]
        public void DataGridCustomComparerTest()
        {
            var comp = Context.RenderComponent<DataGridSelectionComparerTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSelectionComparerTest.Person>>();

            dataGrid.Instance.SelectedItems.Count.Should().Be(0);

            // click the first row
            dataGrid.FindAll("td")[1].Click();
            dataGrid.Instance.SelectedItems.Count.Should().Be(1);
            dataGrid.Instance.Selection.Comparer.Should().BeOfType<DataGridSelectionComparerTest.IdComparer>();

            //select a chip
            var chipSet = comp.FindComponent<MudChipSet<string>>();

            chipSet.FindAll(".mud-chip")[2].Click();
            dataGrid.Instance.SelectedItems.Count.Should().Be(1); //only 1 item is set
            dataGrid.FindAll("input[type=checkbox]").Where(checkbox => checkbox.IsChecked()).ToArray().Length.Should().Be(2); //two items are checked
            dataGrid.Instance.Selection.Comparer.Should().BeOfType<DataGridSelectionComparerTest.RoleComparer>();
        }

        [Test]
        public async Task DataGridSingleSelectionTest()
        {
            var comp = Context.RenderComponent<DataGridSingleSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSingleSelectionTest.Item>>();

            dataGrid.Instance.SelectedItems.Count.Should().Be(0);

            // select first item programmatically
            var firstItem = dataGrid.Instance.Items.ElementAt(0);
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(true, firstItem));
            dataGrid.Instance.SelectedItems.Count.Should().Be(1);
            dataGrid.Instance.SelectedItem.Should().Be(firstItem);

            // select second item programmatically (still should be only one item selected)
            var secondItem = dataGrid.Instance.Items.ElementAt(1);
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(true, secondItem));
            dataGrid.Instance.SelectedItems.Count.Should().Be(1);
            dataGrid.Instance.SelectedItem.Should().Be(secondItem);

            // deselect an item programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(false, secondItem));
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.Instance.SelectedItem.Should().BeNull();

            // nothing should happen as the "select all" shouldn't do anything in single selection mode
            dataGrid.FindAll("input")[0].Change(true);
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
        }

        [Test]
        public async Task DataGridMultiSelectionTest()
        {
            var comp = Context.RenderComponent<DataGridMultiSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridMultiSelectionTest.Item>>();

            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.FindAll("input")[0].Change(true);
            dataGrid.Instance.SelectedItems.Count.Should().Be(4);

            // deselect an item programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(false, dataGrid.Instance.SelectedItems.First()));
            dataGrid.Instance.SelectedItems.Count.Should().Be(3);

            // select an item programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(dataGrid.Instance.Items.First()));
            dataGrid.Instance.SelectedItems.Count.Should().Be(4);

            // deselect all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(false));
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);

            // select all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(true));
            dataGrid.Instance.SelectedItems.Count.Should().Be(4);

            // deselect from the footer
            dataGrid.Find("tfoot input").Change(false);
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
        }

        [Test]
        public void DataGridMultiSelectionTest_Should_Not_Render_Footer_If_ShowInFooter_Is_False()
        {
            var comp = Context.RenderComponent<DataGridMultiSelectionTest>(
                Parameter(nameof(MudBlazor.UnitTests.TestComponents.DataGrid.DataGridMultiSelectionTest.ShowInFooter), false));
            comp.FindAll("td.footer-cell").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridSelectAllWithFilterTest()
        {
            var comp = Context.RenderComponent<DataGridMultiSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridMultiSelectionTest.Item>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4, because: "all four rows shown by default");
            dataGrid.Instance.SelectedItems.Count.Should().Be(0, because: "no selected items by default");

            var twoBFilter = new FilterDefinition<DataGridMultiSelectionTest.Item>
            {
                Column = dataGrid.Instance.RenderedColumns.FirstOrDefault(c => c.PropertyName == "Name"),
                Operator = FilterOperator.String.Equal,
                Value = "B"
            };

            // Add a FilterDefinition to filter where the Name == "B".
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(twoBFilter));

            dataGrid.FindAll("tbody tr").Count.Should().Be(2, because: "two 'B' rows shown per the filter");

            // select-all
            dataGrid.FindAll("input[type=checkbox]")[0].Change(true);
            dataGrid.Instance.SelectedItems.Count.Should().Be(2, because: "only the two 'B' rows that are visible should get selected");

            await comp.InvokeAsync(() => dataGrid.Instance.ClearFiltersAsync());
            dataGrid.Render();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4, because: "all rows should be shown when filter disapplied");
            dataGrid.Instance.SelectedItems.Count.Should().Be(2, because: "selection should not have changed when filter disapplied");
            dataGrid.FindAll("input")[0].IsChecked().Should().BeFalse(because: "select all checkbox should reflect 'not all selected' state");
            dataGrid.FindAll("tfoot input")[0].IsChecked().Should().BeFalse(because: "select all checkbox should reflect 'not all selected' state");

            // ClearFiltersAsync() has cleared the value, so it needs to be set again
            twoBFilter.Value = "B";
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(twoBFilter));
            dataGrid.FindAll("input[type=checkbox]")[0].IsChecked().Should().BeTrue(because: "select all checkbox should reflect 'all selected' state");
            dataGrid.FindAll("tfoot input[type=checkbox]")[0].IsChecked().Should().BeTrue(because: "select all checkbox should reflect 'all selected' state");
        }

        [Test]
        public async Task DataGridServerMultiSelectionTest()
        {
            var comp = Context.RenderComponent<DataGridServerMultiSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerMultiSelectionTest.Item>>();

            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.FindAll("input")[0].Change(true);
            dataGrid.Instance.SelectedItems.Count.Should().Be(3);

            // deselect an item programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(false, dataGrid.Instance.SelectedItems.First()));
            dataGrid.Instance.SelectedItems.Count.Should().Be(2);

            // select an item programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectedItemAsync(dataGrid.Instance.ServerItems.First()));
            dataGrid.Instance.SelectedItems.Count.Should().Be(3);

            // deselect all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(false));
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);

            // deselect all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(true));
            dataGrid.Instance.SelectedItems.Count.Should().Be(3);

            // deselect from the footer
            dataGrid.Find("tfoot input").Change(false);
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
        }

        [Test]
        public void DataGridEditableSelectionTest()
        {
            var comp = Context.RenderComponent<DataGridEditableWithSelectColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridEditableWithSelectColumnTest.Item>>();

            // test that all rows, header and footer have cell with a checkbox
            dataGrid.FindAll("input.mud-checkbox-input").Count().Should().Be(dataGrid.Instance.Items.Count() + 2);

            //test that changing header sets all items selected
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.FindAll("input.mud-checkbox-input")[0].Change(true);
            dataGrid.Instance.SelectedItems.Count.Should().Be(dataGrid.Instance.Items.Count());
            //test that changing footer unselects all items
            dataGrid.FindAll("input.mud-checkbox-input")[^1].Change(false);
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            //test that changing value in each row selects an item in grid
            for (var i = 1; i < dataGrid.Instance.Items.Count(); i++)
            {
                dataGrid.FindAll("input.mud-checkbox-input")[i].Change(true);
                dataGrid.Instance.SelectedItems.Count.Should().Be(i);
            }
        }

        [Test]
        public void DataGridInlineEditVirtualizeServerDataTest()
        {
            var comp = Context.RenderComponent<DataGridCellEditVirtualizeServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditVirtualizeServerDataTest.Item>>();

            dataGrid.FindAll("td input")[0].GetAttribute("value").Trim().Should().Be("John");
            dataGrid.FindAll("td input")[1].GetAttribute("value").Trim().Should().Be("45");
            dataGrid.FindAll("td input")[2].GetAttribute("value").Trim().Should().Be("Johanna");
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("23");
            dataGrid.FindAll("td input")[4].GetAttribute("value").Trim().Should().Be("Steve");
            dataGrid.FindAll("td input")[5].GetAttribute("value").Trim().Should().Be("32");
            dataGrid.FindAll(".mud-table-body tr td input")[0].Change("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].Change(52d);
            dataGrid.FindAll(".mud-table-body tr td input")[0].GetAttribute("value").Trim().Should().Be("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].GetAttribute("value").Trim().Should().Be("52");
        }

        /// <summary>
        /// Ensures that multiple calls to reload the data grid data properly flag the CancellationToken.
        /// </summary>
        /// <returns>A <see cref="Task"/> object.</returns>
        [Test]
        public async Task DataGridVirtualizeServerDataLoadingWithCancelTest()
        {
            var comp = Context.RenderComponent<DataGridVirtualizeServerDataLoadingWithCancelTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<int>>();

            // Make a cancellation token we can monitor
            CancellationToken? cancelToken = null;
            // Make a task completion source
            var first = new TaskCompletionSource<GridData<int>>();
            // Set the ServerData function
            dataGrid.SetParam(p =>
                p.VirtualizeServerData,
                new Func<GridStateVirtualize<int>, CancellationToken, Task<GridData<int>>>((_, cancellationToken) =>
                {
                    // Remember the cancellation token
                    cancelToken = cancellationToken;
                    // Return a task that never completes
                    return first.Task;
                }));

            await Task.Delay(20);

            // Test

            // Make sure this first request was not canceled
            comp.WaitForAssertion(() => cancelToken?.IsCancellationRequested.Should().BeFalse());

            // Arrange a server data refresh
            var second = new TaskCompletionSource<GridData<int>>();
            // Set the VirtualizeServerData function to a new method...
            dataGrid.SetParam(p =>
                p.VirtualizeServerData,
                new Func<GridStateVirtualize<int>, CancellationToken, Task<GridData<int>>>((_, _) => second.Task));

            await Task.Delay(20);

            // Test

            // Make sure this second request DID cancel the first request's token
            comp.WaitForAssertion(() => cancelToken?.IsCancellationRequested.Should().BeTrue());
        }

        [Test]
        public async Task DataGridPaginationTest()
        {
            var comp = Context.RenderComponent<DataGridPaginationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridPaginationTest.Item>>();
            // check that the page size dropdown is shown
            comp.FindComponents<MudSelect<int>>().Count.Should().Be(1);

            dataGrid.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 20");

            // test that we are on the first page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("0");

            // click to go to the next page
            dataGrid.FindAll(".mud-table-pagination-actions button")[2].Click();

            // test that we are on the second page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("10");
            dataGrid.Instance.RowsPerPage.Should().Be(10);

            // set rows per page programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetRowsPerPageAsync(4));
            dataGrid.Instance.RowsPerPage.Should().Be(4);

            // navigate to the last page programmatically
            await comp.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.Last));
            dataGrid.Instance.CurrentPage.Should().Be(4);

            // navigate to the previous page programmatically
            await comp.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.Previous));
            dataGrid.Instance.CurrentPage.Should().Be(3);

            // navigate back to the first page programmatically
            await comp.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.First));
            dataGrid.Instance.CurrentPage.Should().Be(0);
        }

        [Test]
        public void DataGridPaginationPageSizeDropDownTest()
        {
            var comp = Context.RenderComponent<DataGridPaginationTest>(self => self.Add(x => x.PageSizeDropDown, false));
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridPaginationTest.Item>>();
            dataGrid.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 20");

            // test that we are on the first page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("0");

            // page size drop-down is not shown
            comp.FindComponents<MudSelect<string>>().Should().BeEmpty();
        }

        /// <summary>
        /// Tests that the "All" data grid pager option shows all items
        /// </summary>
        [Test]
        public async Task DataGridPagingAllTest()
        {
            var comp = Context.RenderComponent<DataGridPaginationAllItemsTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridPaginationAllItemsTest.Item>>();
            var pager = comp.FindComponent<MudSelect<int>>().Instance;

            dataGrid.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-10 of 20"); //check initial value
            // change page size
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetRowsPerPageAsync(int.MaxValue));
            pager.Value.Should().Be(int.MaxValue);
            dataGrid.Instance.RowsPerPage.Should().Be(int.MaxValue);
            comp.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("1-20 of 20");

            comp.FindAll(".mud-table-pagination-actions button")[0].IsDisabled().Should().Be(true); //buttons are disabled
            comp.FindAll(".mud-table-pagination-actions button")[1].IsDisabled().Should().Be(true);
            comp.FindAll(".mud-table-pagination-actions button")[2].IsDisabled().Should().Be(true);
            comp.FindAll(".mud-table-pagination-actions button")[3].IsDisabled().Should().Be(true);

            comp.FindAll(".mud-table-pagination-select")[^1].TextContent.Trim().Should().Be("All");
        }

        [Test]
        public void DataGridPaginationNoItemsTest()
        {
            var comp = Context.RenderComponent<DataGridPaginationNoItemsTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridPaginationNoItemsTest.Item>>();
            // check that the page size dropdown is shown
            comp.FindComponents<MudSelect<int>>().Count.Should().Be(1);

            dataGrid.FindAll(".mud-table-pagination-caption")[^1].TextContent.Trim().Should().Be("0-0 of 0");
        }

        [Test]
        public void DataGridHideNavigationTest()
        {
            var comp = Context.RenderComponent<DataGridPaginationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridPaginationTest.Item>>();
            var pagerContent = comp.FindComponent<MudDataGridPager<DataGridPaginationTest.Item>>();

            comp.Markup.Should().Contain("mud-table-pagination-actions");
            comp.Markup.Should().Contain("M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z");
            comp.Markup.Should().Contain("1-10 of 20");
            pagerContent.SetParam("ShowNavigation", false);
            comp.Markup.Should().NotContain("mud-table-pagination-actions");
            pagerContent.SetParam("ShowPageNumber", false);
            comp.Markup.Should().NotContain("1-10 of 20");
        }

        [Test]
        public async Task DataGridRowsPerPageTwoWayBindingTest()
        {
            var comp = Context.RenderComponent<DataGridRowsPerPageBindingTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridRowsPerPageBindingTest.Item>>();

            // confirm that BoundRowsPerPage is equal to the initial value of 5 (See DataGridRowsPerPageBindingTest)
            comp.Instance.BoundRowsPerPage.Should().Be(5);

            // programmatically set the datagrid rowsPerPage to 10
            await dataGrid.InvokeAsync(() => dataGrid.Instance.SetRowsPerPageAsync(10));

            // confirm that BoundRowsPerPage changes when rowsPerPage is set to 10
            comp.Instance.BoundRowsPerPage.Should().Be(10);
        }

        [Test]
        public void DataGridInlineEditTest()
        {
            var comp = Context.RenderComponent<DataGridCellEditTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditTest.Model>>();

            dataGrid.FindAll("td input")[0].GetAttribute("value").Trim().Should().Be("John");
            dataGrid.FindAll("td input")[1].GetAttribute("value").Trim().Should().Be("45");
            dataGrid.FindAll("td input")[2].GetAttribute("value").Trim().Should().Be("Johanna");
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("23");
            dataGrid.FindAll("td input")[4].GetAttribute("value").Trim().Should().Be("Steve");
            dataGrid.FindAll("td input")[5].GetAttribute("value").Trim().Should().Be("32");
            dataGrid.FindAll(".mud-table-body tr td input")[0].Change("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].Change(52d);
            dataGrid.FindAll(".mud-table-body tr td input")[0].GetAttribute("value").Trim().Should().Be("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].GetAttribute("value").Trim().Should().Be("52");

            var name = dataGrid.Instance.Items.First().Name;
            var age = dataGrid.Instance.Items.First().Age;
            name.Should().Be("Jonathan");
            age.Should().Be(52);
        }

        [Test]
        public void DataGridInlineEditWithNullableChangeTest()
        {
            var comp = Context.RenderComponent<DataGridCellEditWithNullableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditWithNullableTest.Model>>();

            // try setting a value to null
            dataGrid.FindAll("td input")[1].Change("");
            dataGrid.Instance.Items.First().Age.Should().Be(null);

            // try setting the value back to something not null
            dataGrid.FindAll("td input")[1].Change("15");
            dataGrid.Instance.Items.First().Age.Should().Be(15);
        }

        [Test]
        public void DataGridInlineEditWithNullableTest()
        {
            var comp = Context.RenderComponent<DataGridCellEditWithNullableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditWithNullableTest.Model>>();

            dataGrid.FindAll("td input")[0].GetAttribute("value").Trim().Should().Be("John");
            dataGrid.FindAll("td input")[1].GetAttribute("value").Trim().Should().Be("45");
            dataGrid.FindAll("td input")[2].GetAttribute("value").Trim().Should().Be("Johanna");
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("23");
            dataGrid.FindAll("td input")[4].GetAttribute("value").Trim().Should().Be("Steve");
            dataGrid.FindAll("td input")[5].GetAttribute("value").Should().BeNull();
            dataGrid.FindAll(".mud-table-body tr td input")[0].Change("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].Change(52);
            dataGrid.FindAll(".mud-table-body tr td input")[0].GetAttribute("value").Trim().Should().Be("Jonathan");
            dataGrid.FindAll(".mud-table-body tr td input")[1].GetAttribute("value").Trim().Should().Be("52");

            var name = dataGrid.Instance.Items.First().Name;
            var age = dataGrid.Instance.Items.First().Age;
            name.Should().Be("Jonathan");
            age.Should().Be(52);
        }

        [Test]
        public void DataGridInlineEditWithTemplateTest()
        {
            var comp = Context.RenderComponent<DataGridCellEditWithTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellEditWithTemplateTest.Model>>();

            dataGrid.FindAll("td input")[0].GetAttribute("value").Trim().Should().Be("John");
            dataGrid.FindAll("td input")[1].GetAttribute("value").Trim().Should().Be("45");
            dataGrid.FindAll("td input")[2].HasAttribute("checked").Should().Be(false);
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("Johanna");
            dataGrid.FindAll("td input")[4].GetAttribute("value").Trim().Should().Be("23");
            dataGrid.FindAll("td input")[5].HasAttribute("checked").Should().Be(true);
            dataGrid.FindAll("td input")[6].GetAttribute("value").Trim().Should().Be("Steve");
            dataGrid.FindAll("td input")[7].GetAttribute("value").Trim().Should().Be("32");
            dataGrid.FindAll("td input")[8].HasAttribute("value").Should().Be(false);
            dataGrid.FindAll("td input")[0].Change("Jonathan");
            dataGrid.FindAll("td input")[1].Change(52d);
            dataGrid.FindAll("td input")[2].Change(true);
            dataGrid.FindAll("td input")[0].GetAttribute("value").Trim().Should().Be("Jonathan");
            dataGrid.FindAll("td input")[1].GetAttribute("value").Trim().Should().Be("52");
            dataGrid.FindAll("td input")[2].HasAttribute("checked").Should().Be(true);

            var name = dataGrid.Instance.Items.First().Name;
            var age = dataGrid.Instance.Items.First().Age;
            var hired = dataGrid.Instance.Items.First().Hired;
            name.Should().Be("Jonathan");
            age.Should().Be(52);
            hired.Should().Be(true);
        }

        [Test]
        public void DataGridDialogEditTest()
        {
            var comp = Context.RenderComponent<DataGridFormEditTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFormEditTest.Model>>();

            //verify values before opening dialog
            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("John");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be("45");
            dataGrid.FindAll("td")[2].Html().Trim().Should().Be("snakex64");
            dataGrid.FindAll("td")[3].Html().Trim().Should().Be("Johanna");
            dataGrid.FindAll("td")[4].Html().Trim().Should().Be("23");
            dataGrid.FindAll("td")[5].Html().Trim().Should().Be("snakex64");
            dataGrid.FindAll("td")[6].Html().Trim().Should().Be("Steve");
            dataGrid.FindAll("td")[7].Html().Trim().Should().Be("32");
            dataGrid.FindAll("td")[8].Html().Trim().Should().Be("snakex64");

            //open edit dialog
            dataGrid.FindAll("tbody tr")[1].Click();
            //No close button
            comp.FindAll("button[aria-label=\"Close dialog\"]").Should().BeEmpty();
            //edit data
            comp.FindAll("div input")[0].Change("Galadriel");
            comp.FindAll("div input")[1].Change(1);

            comp.Find(".mud-dialog-actions .mud-button-filled-primary").Click();

            //verify values after saving dialog
            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("John");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be("45");
            dataGrid.FindAll("td")[2].Html().Trim().Should().Be("snakex64");
            dataGrid.FindAll("td")[3].Html().Trim().Should().Be("Galadriel");
            dataGrid.FindAll("td")[4].Html().Trim().Should().Be("1");
            dataGrid.FindAll("td")[5].Html().Trim().Should().Be("snakex64");
            dataGrid.FindAll("td")[6].Html().Trim().Should().Be("Steve");
            dataGrid.FindAll("td")[7].Html().Trim().Should().Be("32");
            dataGrid.FindAll("td")[8].Html().Trim().Should().Be("snakex64");

            //if no crash occurs, we know the datagrid is properly filtering out the GetOnly property when calling set
        }

        [Test(Description = "Checks if there is no NRE exception when nested property has a null value somewhere in the middle.")]
        public void DataGridNoNullExceptionWhenNestedPropertyNullValue()
        {
            var comp = Context.RenderComponent<DataGridNestedNullPropertyTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridNestedNullPropertyTest.Model>>();
            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("Class A");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be(string.Empty);

            var alertTextFunc = () => MudAlert().Find("div.mud-alert-message");
            IRenderedComponent<MudAlert> MudAlert() => comp.FindComponent<MudAlert>();
            alertTextFunc.Should().Throw<ComponentNotFoundException>();
        }

        [Test(Description = "Checks if clone strategy is working, if we used default one it would fail as STJ doesn't support abstract classes without additional configuration.")]
        public void DataGridDialogEditCloneStrategyTest1()
        {
            var comp = Context.RenderComponent<DataGridFormEditCloneStrategyTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFormEditCloneStrategyTest.Movement>>();

            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("James");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be("Robert");
            dataGrid.FindAll("td")[2].Html().Trim().Should().Be("1");
            dataGrid.FindAll("td")[3].Html().Trim().Should().Be("first");
            dataGrid.FindAll("td")[4].Html().Trim().Should().Be("John");
            dataGrid.FindAll("td")[5].Html().Trim().Should().Be("David");
            dataGrid.FindAll("td")[6].Html().Trim().Should().Be("2");
            dataGrid.FindAll("td")[7].Html().Trim().Should().Be("second");

            //open edit dialog
            dataGrid.FindAll("tbody tr")[1].Click();
            //No close button
            comp.FindAll("button[aria-label=\"Close dialog\"]").Should().BeEmpty();
            //edit data
            comp.FindAll("div input")[0].Change("Galadriel");
            comp.FindAll("div input")[1].Change("Steve");
            comp.FindAll("div input")[2].Change("3");

            comp.Find(".mud-dialog-actions .mud-button-filled-primary").Click();

            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("James");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be("Robert");
            dataGrid.FindAll("td")[2].Html().Trim().Should().Be("1");
            dataGrid.FindAll("td")[3].Html().Trim().Should().Be("first");
            dataGrid.FindAll("td")[4].Html().Trim().Should().Be("Galadriel");
            dataGrid.FindAll("td")[5].Html().Trim().Should().Be("Steve");
            dataGrid.FindAll("td")[6].Html().Trim().Should().Be("3");
            dataGrid.FindAll("td")[7].Html().Trim().Should().Be("second");
        }

        [Test]
        public void DataGridDialogEditCloneStrategyTest2()
        {
            var comp = Context.RenderComponent<DataGridFormEditCloneStrategyTest>(parameters => parameters
                .Add(p => p.CloneStrategy, SystemTextJsonDeepCloneStrategy<DataGridFormEditCloneStrategyTest.Movement>.Instance));
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFormEditCloneStrategyTest.Movement>>();

            dataGrid.FindAll("td")[0].Html().Trim().Should().Be("James");
            dataGrid.FindAll("td")[1].Html().Trim().Should().Be("Robert");
            dataGrid.FindAll("td")[2].Html().Trim().Should().Be("1");
            dataGrid.FindAll("td")[3].Html().Trim().Should().Be("first");
            dataGrid.FindAll("td")[4].Html().Trim().Should().Be("John");
            dataGrid.FindAll("td")[5].Html().Trim().Should().Be("David");
            dataGrid.FindAll("td")[6].Html().Trim().Should().Be("2");
            dataGrid.FindAll("td")[7].Html().Trim().Should().Be("second");

            //open edit dialog
            var openDialog = () => dataGrid.FindAll("tbody tr")[1].Click();

            openDialog.Should().Throw<NotSupportedException>("STJ doesn't support abstract classes without polymorphic type discriminators.");
        }

        /// <summary>
        /// DataGrid edit form should trigger the FormFieldChanged event
        /// </summary>
        [Test]
        public void DataGridFormFieldChangedTest()
        {
            var comp = Context.RenderComponent<DataGridFormFieldChangedTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFormFieldChangedTest.Item>>();
            //open edit dialog
            dataGrid.FindAll("tbody tr")[0].Click();

            //edit data
            comp.Find("div input").Change("J K Simmons");
            comp.Instance.FormFieldChangedEventArgs.NewValue.Should().Be("J K Simmons");

            var textfield = comp.FindComponent<MudTextField<string>>();
            textfield.Instance.Should().BeSameAs(comp.Instance.FormFieldChangedEventArgs.Field);
        }

        [Test]
        public void DataGridFormValidationErrorsPreventUpdateTest()
        {
            var comp = Context.RenderComponent<DataGridFormValidationErrorsPreventUpdateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFormValidationErrorsPreventUpdateTest.Model>>();

            // open form dialog
            dataGrid.Find("tbody tr button").Click();
            dataGrid.Instance._isEditFormOpen.Should().BeTrue();

            var field = comp.FindComponents<MudTextField<string>>()[2];

            // edit data
            field.Instance.Value.Should().Be("Augusta_Homenick26@mud.com");
            field.WaitForElement("input").Change("not-a-valid-email-address");

            // check the change occurred
            field.Instance.Value.Should().Be("not-a-valid-email-address");

            // ensure that validation message is displayed
            field.Markup.Should().Contain("This is not a valid e-mail address");

            var button = comp.FindComponents<MudButton>().Single(b => b.Markup.Contains("Save"));
            button.WaitForElement("button").Click();

            // dialog should still be open and the items data should not have been updated
            using AssertionScope scope = new();
            dataGrid.Instance._isEditFormOpen.Should().BeTrue();
            comp.Instance.Items[0].Email.Should().Be("Augusta_Homenick26@mud.com");
        }

        [Test]
        public void DataGridVisualStylingTest()
        {
            var comp = Context.RenderComponent<DataGridVisualStylingTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridVisualStylingTest.Item>>();

            dataGrid.FindAll("td")[1].GetAttribute("style").Should().Contain("background-color:#E5BDE5");
            dataGrid.FindAll("td")[2].GetAttribute("style").Should().Contain("font-weight:bold");

            dataGrid.FindAll("th")[0].GetAttribute("style").Should().Contain("background-color:#E5BDE5");
            dataGrid.FindAll("th")[0].GetAttribute("style").Should().Contain("font-weight:bold");

            dataGrid.FindAll("th")[0].GetAttribute("class").Should().Contain("class-a");
            dataGrid.FindAll("th")[0].GetAttribute("class").Should().Contain("class-b");
            dataGrid.FindAll("th")[0].GetAttribute("class").Should().Contain("class-c");
        }

        [Test]
        public async Task DataGridEventCallbacksTest()
        {
            var comp = Context.RenderComponent<DataGridEventCallbacksTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridEventCallbacksTest.Item>>();

            // Include callbacks in test coverage.
            dataGrid.Instance.RowClick.HasDelegate.Should().Be(true);
            dataGrid.Instance.RowContextMenuClick.HasDelegate.Should().Be(true);
            dataGrid.Instance.SelectedItemChanged.HasDelegate.Should().Be(true);
            dataGrid.Instance.CommittedItemChanges.HasDelegate.Should().Be(true);
            dataGrid.Instance.StartedEditingItem.HasDelegate.Should().Be(true);
            dataGrid.Instance.CanceledEditingItem.HasDelegate.Should().Be(true);
            dataGrid.Instance.CanceledEditingItem.Should().Be(dataGrid.Instance.CanceledEditingItem);

            // we test to make sure that we can set and get the cancelCallback via the CancelledEditingItem property
            var cancelCallback = dataGrid.Instance.CanceledEditingItem;
            dataGrid.SetCallback(dg => dg.CanceledEditingItem, x => { });
            dataGrid.Instance.CanceledEditingItem.Should().NotBe(cancelCallback);
            dataGrid.Instance.CanceledEditingItem = cancelCallback;
            dataGrid.Instance.CanceledEditingItem.Should().Be(cancelCallback);

            // Set some parameters manually so that they are covered.
            var parameters = new[]
            {
                ComponentParameter.CreateParameter(nameof(dataGrid.Instance.MultiSelection), true),
                ComponentParameter.CreateParameter(nameof(dataGrid.Instance.ReadOnly), false),
                ComponentParameter.CreateParameter(nameof(dataGrid.Instance.EditMode), DataGridEditMode.Cell),
                ComponentParameter.CreateParameter(nameof(dataGrid.Instance.EditTrigger), DataGridEditTrigger.OnRowClick)
            };
            dataGrid.SetParametersAndRender(parameters);

            // Make sure that the callbacks have not been fired yet.
            comp.Instance.RowClicked.Should().Be(false);
            comp.Instance.RowContextMenuClicked.Should().Be(false);
            comp.Instance.SelectedItemChanged.Should().Be(false);
            comp.Instance.CommittedItemChanges.Should().Be(false);
            comp.Instance.StartedEditingItem.Should().Be(false);
            comp.Instance.CanceledEditingItem.Should().Be(false);

            // Fire RowClick, SelectedItemChanged, SelectedItemsChanged, and StartedEditingItem callbacks.
            dataGrid.FindAll(".mud-table-body tr")[0].Click();

            // Fire RowContextMenuClick
            dataGrid.FindAll(".mud-table-body tr")[0].ContextMenu();

            // Edit an item.
            dataGrid.FindAll(".mud-table-body tr td input")[0].Change("A test");

            // Make sure that the callbacks have been fired.
            comp.Instance.RowClicked.Should().Be(true);
            comp.Instance.RowContextMenuClicked.Should().Be(true);
            comp.Instance.SelectedItemChanged.Should().Be(true);
            comp.Instance.CommittedItemChanges.Should().Be(true);
            comp.Instance.CanceledEditingItem.Should().Be(false);

            // TODO: Triggering of the CancelEditingItem callback appears to require the Form edit mode
            // but we can brute force it by directly calling the CancelEditingItemAsync method on the datagrid
            await dataGrid.InvokeAsync(dataGrid.Instance.CancelEditingItemAsync);
            comp.Instance.CanceledEditingItem.Should().Be(true);
        }

        [Test]
        public void DataGridEditComplexPropertyExpressionTest()
        {
            var comp = Context.RenderComponent<DataGridEditComplexPropertyExpressionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridEditComplexPropertyExpressionTest.Item>>();


            dataGrid.Render();

            // Make sure that the value is as expected before we try to change it
            comp.Instance.Items[0].Name.Should().Be("A");
            comp.Instance.Items[0].SubItem.SubProperty.Should().Be("A-D");
            comp.Instance.Items[0].SubItem.SubItem2.SubProperty2.Should().Be("A-D-E");

            // Edit an item 'normally'
            dataGrid.FindAll(".mud-table-body tr td input")[0].Change("Test 1");
            comp.Instance.Items[0].Name.Should().Be("Test 1");

            // Edit an item that has a sub property like x.Something.SomethingElse
            dataGrid.FindAll(".mud-table-body tr td input")[1].Change("Test 2");
            comp.Instance.Items[0].SubItem.SubProperty.Should().Be("Test 2");

            // Edit an item that has a sub property like x.Something.SomethingElse.SomethingElseAgain
            dataGrid.FindAll(".mud-table-body tr td input")[2].Change("Test 3");
            comp.Instance.Items[0].SubItem.SubItem2.SubProperty2.Should().Be("Test 3");
        }

        [Test]
        public void DataGridOnContextMenuClickWhenIsGrouped()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedTest.Fruit>>();

            // Include callbacks in test coverage.
            dataGrid.Instance.RowContextMenuClick.HasDelegate.Should().Be(true);

            // Make sure that the callbacks have not been fired yet.
            comp.Instance.RowContextMenuClicked.Should().Be(false);

            // Fire RowContextMenuClick
            dataGrid.FindAll(".mud-table-body tr")[1].ContextMenu();

            // Make sure that the callbacks have been fired.
            comp.Instance.RowContextMenuClicked.Should().Be(true);
        }

        [Test]
        public async Task DataGridServerSideSortableTest()
        {
            // Disable simulated load on server side:
            TestComponents.DataGrid.DataGridServerSideSortableTest.DisableServerTimeoutForTests = true;

            var comp = Context.RenderComponent<DataGridServerSideSortableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerSideSortableTest.Item>>();

            var cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(21, because: "We have 7 data rows with three columns");

            // Check the values of rows without sorting
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted ascending by Name.
            cells[0].TextContent.Should().Be("A"); cells[1].TextContent.Should().Be("73"); cells[2].TextContent.Should().Be("7");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("11"); cells[5].TextContent.Should().Be("4444");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("99"); cells[8].TextContent.Should().Be("66");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("C"); cells[13].TextContent.Should().Be("33"); cells[14].TextContent.Should().Be("33333");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Descending, x => { return x.Name; }));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be sorted descending by A.
            cells[0].TextContent.Should().Be("C"); cells[1].TextContent.Should().Be("33"); cells[2].TextContent.Should().Be("33333");
            cells[3].TextContent.Should().Be("C"); cells[4].TextContent.Should().Be("44"); cells[5].TextContent.Should().Be("1111111");
            cells[6].TextContent.Should().Be("C"); cells[7].TextContent.Should().Be("55"); cells[8].TextContent.Should().Be("222222");
            cells[9].TextContent.Should().Be("B"); cells[10].TextContent.Should().Be("42"); cells[11].TextContent.Should().Be("555");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("73"); cells[14].TextContent.Should().Be("7");
            cells[15].TextContent.Should().Be("A"); cells[16].TextContent.Should().Be("11"); cells[17].TextContent.Should().Be("4444");
            cells[18].TextContent.Should().Be("A"); cells[19].TextContent.Should().Be("99"); cells[20].TextContent.Should().Be("66");

            await comp.InvokeAsync(() => dataGrid.Instance.RemoveSortAsync("Name"));
            cells = dataGrid.FindAll("td");

            // Check the values of rows - should be the default order of the items.
            cells[0].TextContent.Should().Be("B"); cells[1].TextContent.Should().Be("42"); cells[2].TextContent.Should().Be("555");
            cells[3].TextContent.Should().Be("A"); cells[4].TextContent.Should().Be("73"); cells[5].TextContent.Should().Be("7");
            cells[6].TextContent.Should().Be("A"); cells[7].TextContent.Should().Be("11"); cells[8].TextContent.Should().Be("4444");
            cells[9].TextContent.Should().Be("C"); cells[10].TextContent.Should().Be("33"); cells[11].TextContent.Should().Be("33333");
            cells[12].TextContent.Should().Be("A"); cells[13].TextContent.Should().Be("99"); cells[14].TextContent.Should().Be("66");
            cells[15].TextContent.Should().Be("C"); cells[16].TextContent.Should().Be("44"); cells[17].TextContent.Should().Be("1111111");
            cells[18].TextContent.Should().Be("C"); cells[19].TextContent.Should().Be("55"); cells[20].TextContent.Should().Be("222222");

            await comp.InvokeAsync(() => dataGrid.Instance.SortMode = SortMode.None);
            dataGrid.Render();
            dataGrid.Instance.DropContainerHasChanged();
            // Since Sortable is now false, the click handler (and element holding it) should no longer exist.
            dataGrid.FindAll(".column-header .sortable-column-header").Should().BeEmpty();
        }

        [Test]
        public void FilterDefinitionStringTest()
        {
            var comp = Context.RenderComponent<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            var nameColumn = dataGrid.Instance.GetColumnByPropertyName("Name");

            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>();
            Func<DataGridFiltersTest.Model, bool> func = null;

            #region FilterOperator.String.Contains

            //default Case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Contains,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();

            //case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Contains,
                Value = "Joe"
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();

            // null value default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Contains,
                Value = null,
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeTrue();

            // null value default case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Contains,
                Value = null
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.String.NotContains

            // default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotContains,
                Value = "Joe"
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();

            // case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotContains,
                Value = "Joe"
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotContains,
                Value = null
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Does not contain", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.String.Equal

            //default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Equal,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeFalse();

            //case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Equal,
                Value = "Joe"
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeTrue();

            // null value default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Equal,
                Value = null,
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeTrue();

            // null value case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Equal,
                Value = null
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.String.NotEqual

            //default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotEqual,
                Value = "Joe"
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeTrue();

            //case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotEqual,
                Value = "Joe"
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotEqual,
                Value = null
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.String.StartsWith

            //default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.StartsWith,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeFalse();

            //case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.StartsWith,
                Value = "Joe"
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.StartsWith,
                Value = null
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Not Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.String.EndsWith

            //default case sensitivity
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.EndsWith,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeFalse();

            //case insensitive
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.EndsWith,
                Value = "Joe"
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.CaseInsensitive));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("joe", 45, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.EndsWith,
                Value = null
            };
            dataGrid.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.FilterCaseSensitivity, DataGridFilterCaseSensitivity.Default));
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.String.Empty

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.Empty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(string.Empty, 45, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotEmpty,
                Value = null,
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(string.Empty, 45, null, null, null, null)).Should().BeFalse();

            #endregion

            #region FilterOperator.String.NotEmpty

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotEmpty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(string.Empty, 45, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = FilterOperator.String.NotEmpty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("", 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeFalse();
            func.Invoke(new(string.Empty, 45, null, null, null, null)).Should().BeFalse();

            #endregion

            // handle null operator
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = nameColumn,
                Operator = null,
                Value = "Joe"
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Joe Not", 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new(null, 45, null, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
        }

        [Test]
        public void FilterDefinitionBoolTest()
        {
            var comp = Context.RenderComponent<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            var hiredColumn = dataGrid.Instance.GetColumnByPropertyName("Hired");

            #region FilterOperator.Boolean.Is

            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = hiredColumn,
                Operator = FilterOperator.Boolean.Is,
                Value = true
            };
            var func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, false, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, true, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = hiredColumn,
                Operator = FilterOperator.Boolean.Is,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, false, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, true, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = hiredColumn,
                Operator = null,
                Value = true
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, false, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, true, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
        }

        [Test]
        public void FilterDefinitionEnumTest()
        {
            var comp = Context.RenderComponent<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            var statusColumn = dataGrid.Instance.GetColumnByPropertyName("Status");

            #region FilterOperator.Enum.Is

            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = statusColumn,
                Operator = FilterOperator.Enum.Is,
                Value = Severity.Normal
            };
            var func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 456, Severity.Info, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, Severity.Normal, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = statusColumn,
                Operator = FilterOperator.Enum.Is,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 456, Severity.Info, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, Severity.Normal, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.Enum.IsNot

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = statusColumn,
                Operator = FilterOperator.Enum.IsNot,
                Value = Severity.Normal
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 456, Severity.Normal, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, Severity.Info, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = statusColumn,
                Operator = FilterOperator.Enum.IsNot,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 456, Severity.Normal, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, Severity.Info, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = statusColumn,
                Operator = null,
                Value = Severity.Normal
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 456, Severity.Normal, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, Severity.Info, null, null, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
        }

        [Test]
        public void FilterDefinitionDateTimeTest()
        {
            var comp = Context.RenderComponent<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            var dateColumn = dataGrid.Instance.GetColumnByPropertyName("HiredOn");
            var utcnow = DateTime.UtcNow;

            #region FilterOperator.DateTime.Is

            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.Is,
                Value = utcnow
            };
            var func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.Is,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.IsNot

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.IsNot,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.IsNot,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.After

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.After,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.After,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.OnOrAfter

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.OnOrAfter,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.OnOrAfter,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.Before

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.Before,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.Before,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.OnOrBefore

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.OnOrBefore,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.OnOrBefore,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.Empty

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.Empty,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.Empty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.DateTime.NotEmpty

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.NotEmpty,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = FilterOperator.DateTime.NotEmpty,
                Value = null
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dateColumn,
                Operator = null,
                Value = utcnow
            };
            func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 45, null, null, utcnow, null)).Should().BeTrue();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
        }

        [Test]
        public void FilterDefinitionNumberTest()
        {
            var comp = Context.RenderComponent<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            var ageColumn = dataGrid.Instance.GetColumnByPropertyName("Age");

            #region FilterOperator.Number.Equal

            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.Equal,
                Value = 45
            };
            var func = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func.Invoke(new("Sam", 456, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Sam", null, null, null, null, null)).Should().BeFalse();
            func.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.Equal,
                Value = null
            };
            var func2 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            // data type is an int
            func2.Invoke(new("Sam", 456, null, null, null, null)).Should().BeTrue();
            func2.Invoke(new("Sam", null, null, null, null, null)).Should().BeTrue();
            func2.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.Number.NotEqual

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.NotEqual,
                Value = 45
            };
            var func3 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func3.Invoke(new("Sam", 456, null, null, null, null)).Should().BeTrue();
            func3.Invoke(new("Sam", null, null, null, null, null)).Should().BeTrue();
            func3.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.NotEqual,
                Value = null
            };
            var func4 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func4.Invoke(new("Sam", 456, null, null, null, null)).Should().BeTrue();
            func4.Invoke(new("Sam", null, null, null, null, null)).Should().BeTrue();
            func4.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.Number.GreaterThan

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.GreaterThan,
                Value = 45
            };
            var func5 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func5.Invoke(new("Sam", 456, null, null, null, null)).Should().BeTrue();
            func5.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();
            func5.Invoke(new("Joe", null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.GreaterThan,
                Value = null
            };
            var func6 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func6.Invoke(new("Sam", 456, null, null, null, null)).Should().BeTrue();
            func6.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func6.Invoke(new("Joe", null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.Number.LessThan

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.LessThan,
                Value = 45
            };
            var func7 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func7.Invoke(new("Sam", 4, null, null, null, null)).Should().BeTrue();
            func7.Invoke(new("Joe", 45, null, null, null, null)).Should().BeFalse();
            func7.Invoke(new("Joe", null, null, null, null, null)).Should().BeFalse();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.LessThan,
                Value = null
            };
            var func8 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func8.Invoke(new("Sam", 4, null, null, null, null)).Should().BeTrue();
            func8.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
            func8.Invoke(new("Joe", null, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.Number.GreaterThanOrEqual

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.GreaterThanOrEqual,
                Value = 45
            };
            var func9 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func9.Invoke(new("Sam", 4, null, null, null, null)).Should().BeFalse();
            func9.Invoke(new("Sam", null, null, null, null, null)).Should().BeFalse();
            func9.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.GreaterThanOrEqual,
                Value = null
            };
            var func10 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func10.Invoke(new("Sam", 4, null, null, null, null)).Should().BeTrue();
            func10.Invoke(new("Sam", null, null, null, null, null)).Should().BeTrue();
            func10.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            #region FilterOperator.Number.LessThanOrEqual

            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.LessThanOrEqual,
                Value = 45
            };
            var func11 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func11.Invoke(new("Sam", 46, null, null, null, null)).Should().BeFalse();
            func11.Invoke(new("Sam", null, null, null, null, null)).Should().BeFalse();
            func11.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            // null value
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = FilterOperator.Number.LessThanOrEqual,
                Value = null
            };
            var func12 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func12.Invoke(new("Sam", 46, null, null, null, null)).Should().BeTrue();
            func12.Invoke(new("Sam", null, null, null, null, null)).Should().BeTrue();
            func12.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();

            #endregion

            // null operator
            filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = ageColumn,
                Operator = null,
                Value = 45
            };
            var func13 = filterDefinition.GenerateFilterFunction(new FilterOptions
            {
                FilterCaseSensitivity = dataGrid.Instance.FilterCaseSensitivity
            });
            func13.Invoke(new("Sam", 456, null, null, null, null)).Should().BeTrue();
            func13.Invoke(new("Sam", null, null, null, null, null)).Should().BeTrue();
            func13.Invoke(new("Joe", 45, null, null, null, null)).Should().BeTrue();
        }

        [Test]
        public async Task FilterDefinitionReplaceWithCustom()
        {
            var comp = Context.RenderComponent<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            dataGrid.Instance.SetDefaultFilterDefinition<CustomFilterDefinitionMock<DataGridFiltersTest.Model>>();

            await comp.InvokeAsync(() => dataGrid.Instance.OpenFilters());

            // add a filter via the AddFilter method
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilter());

            // check the number of filters displayed in the filters panel
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(1);

            var filterDefinitionInstance = dataGrid.Instance.FilterDefinitions.FirstOrDefault();
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
            filterDefinitionInstance.Should().NotBeNull();
            filterDefinitionInstance.Should().BeOfType<CustomFilterDefinitionMock<DataGridFiltersTest.Model>>();
        }

        [Test]
        public void DataGridClickFilterButtonTest()
        {
            var comp = Context.RenderComponent<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            IElement FilterButton() => dataGrid.FindAll(".filter-button")[0];

            // click on the filter button
            FilterButton().Click();

            // check the number of filters displayed in the filters panel is 1
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(1);

            // click again on the filter button
            FilterButton().Click();

            // check the number of filters displayed in the filters panel is still 1 (no duplicate filter)
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridCloseFiltersTest()
        {
            var comp = Context.RenderComponent<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();
            IElement FilterButton() => dataGrid.FindAll(".filter-button")[0];

            // click on the filter button
            FilterButton().Click();

            // check the number of filters displayed in the filters panel is 1
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(1);

            await comp.Find(".filter-operator").PointerDownAsync(new PointerEventArgs());

            //set operator to CONTAINS
            comp.FindAll(".mud-list .mud-list-item")[0].Click();
            comp.Find(".mud-overlay").Click();
            comp.Render();

            //should be removed since no value is provided
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0);

            //set operator to NOT CONTAINS
            FilterButton().Click();

            await comp.Find(".filter-operator").PointerDownAsync(new PointerEventArgs());

            comp.FindAll(".mud-list .mud-list-item")[1].Click();
            comp.Find(".mud-overlay").Click();
            comp.Render();

            //should be removed since no value is provided
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0);

            //set operator to EQUALS
            FilterButton().Click();

            await comp.Find(".filter-operator").PointerDownAsync(new PointerEventArgs());

            comp.FindAll(".mud-list .mud-list-item")[2].Click();
            comp.Find(".mud-overlay").Click();
            comp.Render();

            //should be removed since no value is provided
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0);

            //set operator to NOT EQUALS
            FilterButton().Click();

            await comp.Find(".filter-operator").PointerDownAsync(new PointerEventArgs());

            comp.FindAll(".mud-list .mud-list-item")[3].Click();
            comp.Find(".mud-overlay").Click();
            comp.Render();

            //should be removed since no value is provided
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0);

            //set operator to STARTS WITH
            FilterButton().Click();

            await comp.Find(".filter-operator").PointerDownAsync(new PointerEventArgs());

            comp.FindAll(".mud-list .mud-list-item")[4].Click();
            comp.Find(".mud-overlay").Click();
            comp.Render();

            //should be removed since no value is provided
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0);

            //set operator to ENDS WITH
            FilterButton().Click();

            await comp.Find(".filter-operator").PointerDownAsync(new PointerEventArgs());

            comp.FindAll(".mud-list .mud-list-item")[5].Click();
            comp.Find(".mud-overlay").Click();
            comp.Render();

            //should be removed since no value is provided
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0);

            //set operator to IS EMPTY
            FilterButton().Click();

            await comp.Find(".filter-operator").PointerDownAsync(new PointerEventArgs());

            comp.FindAll(".mud-list .mud-list-item")[6].Click();
            comp.Find(".mud-overlay").Click();
            comp.Render();

            //should maintain filter, no value is required
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);

            //set operator to IS NOT EMPTY
            FilterButton().Click();

            await comp.Find(".filter-operator").PointerDownAsync(new PointerEventArgs());

            comp.FindAll(".mud-list .mud-list-item")[7].Click();
            comp.Find(".mud-overlay").Click();
            comp.Render();

            //should maintain filter, no value is required
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridFiltersTest()
        {
            var comp = Context.RenderComponent<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();

            // test filter definition on the Name property (string contains)
            var filterDefinition = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Name"),
                Operator = "contains",
                Value = "John"
            };
            // test filter definition on the Age property (int >)
            var filterDefinition2 = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Age"),
                Operator = ">",
                Value = 30
            };
            // test filter definition on the Status property (Enum is)
            var filterDefinition3 = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Status"),
                Operator = "is",
                Value = Severity.Normal
            };
            // test filter definition on the Hired property (Bool is)
            var filterDefinition4 = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Hired"),
                Operator = "is",
                Value = true
            };
            // test filter definition on the HiredOn property (DateTime is)
            var filterDefinition5 = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("HiredOn"),
                Operator = "is",
                Value = DateTime.UtcNow.Date
            };
            // test filter definition on the ApplicationId property (Guid equals)
            var filterDefinition6 = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("ApplicationId"),
                Operator = "equals",
                Value = Guid.NewGuid()
            };

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(filterDefinition));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(filterDefinition2));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(filterDefinition3));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(filterDefinition4));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(filterDefinition5));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(filterDefinition6));
            await comp.InvokeAsync(() => dataGrid.Instance.OpenFilters());

            // check the number of filters displayed in the filters panel
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(6);

            // click the Add Filter button in the filters panel to add a filter
            comp.FindAll(".filters-panel > button")[0].Click();

            // check the number of filters displayed in the filters panel is 1 more because we added a filter
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(7);

            // add a filter via the AddFilter method
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilter());

            // check the number of filters displayed in the filters panel is 1 more because we added a filter
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(8);

            // add a filter via the AddFilter method
            //await comp.InvokeAsync(() => dataGrid.Instance.AddFilter(Guid.NewGuid(), "Status"));
            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.RenderedColumns.FirstOrDefault(x => x.PropertyName == "Status")
            }));

            // check the number of filters displayed in the filters panel is 1 more because we added a filter
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(9);

            // toggle the filters menu (should be closed after this)
            await comp.InvokeAsync(() => dataGrid.Instance.ToggleFiltersMenu());
            comp.FindAll(".filters-panel").Count.Should().Be(0);

            // test internal filter class for string data type.
            var internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, filterDefinition, null);
            filterDefinition.Column.dataType.Should().Be(typeof(string));
            await comp.InvokeAsync(() => internalFilter.StringValueChanged("J"));
            filterDefinition.Value.Should().Be("J");
            // test internal filter class for number data type.
            internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, filterDefinition2, null);
            filterDefinition2.Column.dataType.Should().Be(typeof(int?));
            await comp.InvokeAsync(() => internalFilter.NumberValueChanged(35));
            filterDefinition2.Value.Should().Be(35);
            // test internal filter class for enum data type.
            internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, filterDefinition3, null);
            filterDefinition3.Column.dataType.Should().Be(typeof(Severity?));
            await comp.InvokeAsync(() => internalFilter.NumberValueChanged(35));
            filterDefinition3.Value.Should().Be(35);
            filterDefinition3.FieldType.IsEnum.Should().Be(true);
            // test internal filter class for bool data type.
            internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, filterDefinition4, null);
            filterDefinition4.Column.dataType.Should().Be(typeof(bool?));
            await comp.InvokeAsync(() => internalFilter.BoolValueChanged(false));
            filterDefinition4.Value.Should().Be(false);
            // test internal filter class for datetime data type.
            var date = DateTime.UtcNow;
            internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, filterDefinition5, null);
            filterDefinition5.Column.dataType.Should().Be(typeof(DateTime?));
            await comp.InvokeAsync(() => internalFilter.DateValueChanged(date));
            filterDefinition5.Value.Should().Be(date.Date);
            await comp.InvokeAsync(() => internalFilter.TimeValueChanged(date.TimeOfDay));
            filterDefinition5.Value.Should().Be(date);
            // test internal filter class for guid data type.
            var guid = Guid.NewGuid();
            internalFilter = new Filter<DataGridFiltersTest.Model>(dataGrid.Instance, filterDefinition6, null);
            filterDefinition6.Column.dataType.Should().Be(typeof(Guid?));
            await comp.InvokeAsync(() => internalFilter.GuidValueChanged(guid));
            filterDefinition6.Value.Should().Be(guid);
        }

        [Test]
        public void DataGridFilterPerColumnTest()
        {
            var comp = Context.RenderComponent<DataGridFilterPerColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterPerColumnTest.Model>>();

            IElement FirstnameFilterButton() => dataGrid.FindAll(".filter-button")[0];

            // click on the filter button
            FirstnameFilterButton().Click();

            // check the number of filters displayed in the filters panel is 1
            comp.FindAll(".filters-panel .mud-grid-item.d-flex").Count.Should().Be(1);

            // get select menus
            var selects = comp.FindAll(".filters-panel .mud-grid-item .mud-input-control.mud-select");
            selects.Count.Should().Be(2);

            // open operator menu
            selects[1].PointerDown();

            //check available operators
            var items = comp.FindAll("div.mud-list-item");

            items.Count.Should().Be(4);
            items.ToMarkup()
                 .Should().Contain("starts with")
                 .And.Contain("ends with")
                 .And.Contain("equals")
                 .And.Contain("contains");
        }

        [Test]
        public void DataGridInvalidFilterPerColumnTest()
        {
            var exception = Assert.Throws<ArgumentException>(() => Context.RenderComponent<DataGridFilterPerColumnTest>(parameters => parameters.Add(x => x.AddInvalid, true)));

            exception.Message.Should().Be("Invalid filter operators for Severity: <");
        }

        [Test]
        public async Task DataGridIDictionaryFiltersTest()
        {
            var comp = Context.RenderComponent<DataGridIDictionaryFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<IDictionary<string, object>>>();

            // test filter definition on the Name property (string contains)
            var filterDefinition = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                //Field = "Name",
                Operator = "contains",
                Value = "John"
            };
            // test filter definition on the Age property (int >)
            var filterDefinition2 = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                //Field = "Age",
                Operator = ">",
                Value = 30
            };
            // test filter definition on the Status property (Enum is)
            var filterDefinition3 = new FilterDefinition<IDictionary<string, object>>
            {
                Id = Guid.NewGuid(),
                //Field = "Status",
                Operator = "is",
                Value = Severity.Normal
            };

            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(filterDefinition));
            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(filterDefinition2));
            await comp.InvokeAsync(() => dataGrid.Instance.FilterDefinitions.Add(filterDefinition3));
            await comp.InvokeAsync(() => dataGrid.Instance.OpenFilters());

            var filters = dataGrid.Instance.FilterDefinitions;

            // assertions for string
            filters[0].Id.Should().Be(filterDefinition.Id);
            filters[0].Operator.Should().Be(filterDefinition.Operator);
            filters[0].Value.Should().Be(filterDefinition.Value);
            filters[0].Value = "Not Joe";
            filterDefinition.Value.Should().Be("Not Joe");

            // assertions for int
            filters[1].Id.Should().Be(filterDefinition2.Id);
            filters[1].Operator.Should().Be(filterDefinition2.Operator);
            filters[1].Value.Should().Be(filterDefinition2.Value);
            filters[1].Value = 45;
            filterDefinition2.Value.Should().Be(45);

            // assertions for Enum
            filters[2].Id.Should().Be(filterDefinition3.Id);
            filters[2].Operator.Should().Be(filterDefinition3.Operator);
            filters[2].Value.Should().Be(filterDefinition3.Value);
            filters[2].Value = Severity.Error;
            filterDefinition3.Value.Should().Be(Severity.Error);
        }

        [Test]
        public void DataGridColGroupTest()
        {
            var comp = Context.RenderComponent<DataGridColGroupTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColGroupTest.Model>>();

            dataGrid.FindAll("col").Count.Should().Be(3);
        }

        [Test]
        public async Task DataGridColReorderRowFiltersTest()
        {
            var comp = Context.RenderComponent<DataGridColReorderRowFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColReorderRowFiltersTest.Model>>();

            await comp.InvokeAsync(() =>
            {
                // Should have 4 entries, 2 headers and an extra
                dataGrid.FindAll("tr").Count.Should().Be(7);

                var switchButton = dataGrid.Find("button.switch-button");
                switchButton.Click();

                var filterHeaders = dataGrid.FindAll("input");
                var ageFilter = filterHeaders[0];
                var nameFilter = filterHeaders[1];

                ageFilter.Input(27);
                // Should have 1 entry + 3
                dataGrid.FindAll("tr").Count.Should().Be(4);

                dataGrid.Instance.ClearFiltersAsync();
                nameFilter.Input("a");
                // Should have 3 entries + 3
                dataGrid.FindAll("tr").Count.Should().Be(6);
            });
        }

        [Test]
        public async Task DataGridColReorderRowModifiedFiltersTest()
        {
            var comp = Context.RenderComponent<DataGridColReorderRowFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColReorderRowFiltersTest.Model>>();

            await comp.InvokeAsync(async () =>
            {
                // Should have 4 entries, 2 headers and an extra
                dataGrid.FindAll("tr").Count.Should().Be(7);

                var ageCol = dataGrid.Instance.RenderedColumns.First(c => c.PropertyName == "Age");
                var modifiedAgeFilter = ageCol.FilterContext.FilterDefinition;
                modifiedAgeFilter.Operator = ">";

                var nameCol = dataGrid.Instance.RenderedColumns.First(c => c.PropertyName == "Name");
                var modifiedNameFilter = nameCol.FilterContext.FilterDefinition;
                modifiedNameFilter.Operator = "not contains";

                await dataGrid.Instance.AddFilterAsync(modifiedAgeFilter);
                await dataGrid.Instance.AddFilterAsync(modifiedNameFilter);

                var switchButton = dataGrid.Find("button.switch-button");
                switchButton.Click();

                var filterHeaders = dataGrid.FindAll("input");
                var ageFilter = filterHeaders[0];
                var nameFilter = filterHeaders[1];

                ageFilter.Input(27);
                // Should have 3 entries + 3
                dataGrid.FindAll("tr").Count.Should().Be(6);

                nameFilter.Input("a");
                // Should have 1 entry + 3
                dataGrid.FindAll("tr").Count.Should().Be(4);
            });
        }

        [Test]
        public void DataGridHeaderTemplateTest()
        {
            var comp = Context.RenderComponent<DataGridHeaderTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHeaderTemplateTest.Model>>();

            dataGrid.Find("thead th").TextContent.Trim().Should().Be("test");

            dataGrid.Find("span.column-header").FirstChild.NodeName.Should().Be("svg");
            dataGrid.Find("span.column-header span").TextContent.Should().Be("Name");
        }

        [Test]
        public async Task DataGridRowDetailOpenTest()
        {
            var comp = Context.RenderComponent<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            await comp.InvokeAsync(() => dataGrid.Instance
            .ToggleHierarchyVisibilityAsync(dataGrid.Instance.Items.First()));

            dataGrid.FindAll("td")[5].TextContent.Trim().Should().StartWith("uid = Sam|56|Normal|");
        }

        [Test]
        public void DataGridRowDetailClosedTest()
        {
            var comp = Context.RenderComponent<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            dataGrid.FindAll("td").SingleOrDefault(x => x.TextContent.Trim().StartsWith("uid = Sam|56|Normal|")).Should().BeNull();
        }

        [Test]
        public void DataGridRowDetailButtonDisabledTest()
        {
            var comp = Context.RenderComponent<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            dataGrid.FindAll("button")[10].OuterHtml.Contains("disabled")
                .Should().BeTrue();
        }

        [Test]
        public async Task DataGridRowDetailButtonDisabledClickTest()
        {
            var comp = Context.RenderComponent<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            await comp.InvokeAsync(() =>
            {
                var buttons = dataGrid.FindAll("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon");
                buttons[10].Click();

                dataGrid.FindAll("td")
                .SingleOrDefault(x => x.TextContent.Trim().StartsWith("uid = Alicia|54|Info|")).Should().BeNull();
            });
        }

        [Test]
        public void DataGridRowDetailInitiallyExpandedMultipleTest()
        {
            var comp = Context.RenderComponent<DataGridHierarchyColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridHierarchyColumnTest.Model>>();

            var item = dataGrid.Instance.Items.FirstOrDefault(x => x.Name == "Ira");

            dataGrid.Instance._openHierarchies.Should().Contain(item);

            item = dataGrid.Instance.Items.FirstOrDefault(x => x.Name == "Anders");

            dataGrid.Instance._openHierarchies.Should().Contain(item);

            comp.Markup.Should().Contain("uid = Ira|27|Success|");
            comp.Markup.Should().Contain("uid = Anders|24|Error|");

            comp.Markup.Should().NotContain("uid = Sam|56|Normal|");
            comp.Markup.Should().NotContain("uid = Alicia|54|Info|");
            comp.Markup.Should().NotContain("uid = John|32|Warning|");
        }

        [Test]
        public void DataGridChildRowContentTest()
        {
            var comp = Context.RenderComponent<DataGridChildRowContentTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridChildRowContentTest.Model>>();

            dataGrid.FindAll("td").SingleOrDefault(x => x.TextContent.Trim().StartsWith("uid = Sam|56|Normal|")).Should().NotBeNull();
        }

        [Test]
        public void DataGridLoadingTest()
        {
            var comp = Context.RenderComponent<DataGridLoadingTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridLoadingTest.Model>>();

            dataGrid.Find("th.mud-table-empty-row div").TextContent.Trim().Should().Be("Data loading, please wait...");
        }

        [Test]
        public void DataGridNoRecordsContentTest()
        {
            var comp = Context.RenderComponent<DataGridNoRecordsContentTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridNoRecordsContentTest.Model>>();

            dataGrid.Find("th.mud-table-empty-row div").TextContent.Trim().Should().Be("There are no records to view.");
        }

        [Test]
        public void DataGridFooterTemplateTest()
        {
            var comp = Context.RenderComponent<DataGridFooterTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFooterTemplateTest.Model>>();

            dataGrid.FindAll("tfoot td").First().TextContent.Trim().Should().Be("Names: Sam, Alicia, Ira, John");
            dataGrid.FindAll("tfoot td").Last().TextContent.Trim().Should().Be($"Highest: {132000:C0} | 2 Over {100000:C0}");
        }

        [Test]
        public async Task DataGridServerPaginationTest()
        {
            var comp = Context.RenderComponent<DataGridServerPaginationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerPaginationTest.Model>>();

            // test that we are on the first page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("Name 0");

            // click to go to the next page
            dataGrid.FindAll(".mud-table-pagination-actions button")[2].Click();

            // test that we are on the second page of results
            dataGrid.Find(".mud-table-body td").TextContent.Trim().Should().Be("Name 10");

            // test reloading server side results programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.ReloadServerData());
        }

        /// <summary>
        /// https://github.com/MudBlazor/MudBlazor/issues/8298
        /// </summary>
        [Test]
        public async Task SetRowsPerPageAsync_CallOneTimeServerData()
        {
            // Arrange

            var comp = Context.RenderComponent<DataGridServerPaginationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerPaginationTest.Model>>();
            dataGrid.Instance.CurrentPage = 2;
            var serverDataCallCount = 0;
            var originalServerDataFunc = dataGrid.Instance.ServerData;
            dataGrid.Instance.ServerData = (state) =>
            {
                serverDataCallCount++;
                return originalServerDataFunc(state);
            };

            // Act

            await dataGrid.InvokeAsync(() => dataGrid.Instance.SetRowsPerPageAsync(25));

            // Assert

            serverDataCallCount.Should().Be(1);
        }

        [Test]
        public void DataGridCellTemplateTest()
        {
            var comp = Context.RenderComponent<DataGridCellTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellTemplateTest.Model>>();

            dataGrid.FindAll("td")[0].TextContent.Trim().Should().Be("John");
            dataGrid.FindAll("td")[1].TextContent.Trim().Should().Be("45");
        }

        [Test]
        public async Task DataGridColumnChooserTest()
        {
            var comp = Context.RenderComponent<DataGridColumnChooserTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnChooserTest.Model>>();
            var popoverProvider = comp.FindComponent<MudPopoverProvider>();
            var popover = dataGrid.FindComponent<MudPopover>();

            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(6);
            await comp.InvokeAsync(() =>
            {
                var columnHamburger = dataGrid.FindAll("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-icon-button-size-small");
                columnHamburger[2].Click();

                var listItems = popoverProvider.FindComponents<MudListItem<object>>();
                listItems.Count.Should().Be(2);
                var clickablePopover = listItems[1].Find(".mud-list-item");
                clickablePopover.Click();

                //dataGrid.Instance._columns[0].Hide();
                ((IMudStateHasChanged)dataGrid.Instance).StateHasChanged();
            });
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(5);
            await comp.InvokeAsync(() =>
            {
                var columnsButton = dataGrid.Find("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-icon-button-size-small");
                columnsButton.Click();

                popover.Instance.Open.Should().BeTrue("Should be open once clicked");
                var listItems = popoverProvider.FindComponents<MudListItem<object>>();
                listItems.Count.Should().Be(1);
                var clickablePopover = listItems[0].Find(".mud-list-item");
                clickablePopover.Click();

                var switches = comp.FindComponents<MudSwitch<bool>>();
                switches.Count.Should().Be(6);

                var iconbuttons = comp.FindComponents<MudIconButton>();
                iconbuttons.Count.Should().Be(29);


                var buttons = comp.FindComponents<MudButton>();
                // this is the show all button
                buttons[1].Find("button").Click();
                // 2 columns, 0 hidden
                comp.FindAll(".mud-table-head th").Count.Should().Be(6);

                //dataGrid.Instance._columns[0].Hide();
                ((IMudStateHasChanged)dataGrid.Instance).StateHasChanged();
            });
            comp.FindAll(".mud-table-head th").Count.Should().Be(6);

            await comp.InvokeAsync(() => dataGrid.Instance.ShowColumnsPanel());
            comp.FindAll(".mud-data-grid-columns-panel").Count.Should().Be(1);
            await comp.InvokeAsync(() => dataGrid.Instance.HideColumnsPanel());
            comp.FindAll(".mud-data-grid-columns-panel").Count.Should().Be(0);

            await comp.InvokeAsync(() => dataGrid.Instance.HideAllColumnsAsync());
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(3);
            await comp.InvokeAsync(() => dataGrid.Instance.ShowAllColumnsAsync());
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(6);
        }

        [Test]
        public async Task DataGridColumnHiddenTest()
        {
            var comp = Context.RenderComponent<DataGridColumnHiddenTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnHiddenTest.Model>>();

            //Console.WriteLine(dataGrid.FindAll(".mud-table-head th").ToMarkup());

            var popoverProvider = comp.FindComponent<MudPopoverProvider>();
            var popover = dataGrid.FindComponent<MudPopover>();
            popover.Instance.Open.Should().BeFalse("Should start as closed");

            var columnsButton = dataGrid.Find("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-icon-button-size-small");
            columnsButton.Click();

            popover.Instance.Open.Should().BeTrue("Should be open once clicked");
            var listItems = popoverProvider.FindComponents<MudListItem<object>>();
            listItems.Count.Should().Be(1);
            var clickablePopover = listItems[0].Find(".mud-list-item");
            clickablePopover.Click();

            // at this point, the column picker should be open
            var switches = comp.FindComponents<MudSwitch<bool>>();
            switches.Count.Should().Be(6);

            switches[0].Instance.Value.Should().BeFalse();
            switches[1].Instance.Value.Should().BeTrue();
            switches[2].Instance.Value.Should().BeFalse();
            switches[3].Instance.Value.Should().BeFalse();
            switches[4].Instance.Value.Should().BeFalse();
            switches[0].Instance.Value.Should().BeFalse();

            var buttons = comp.FindComponents<MudButton>();

            // this is the hide all button
            buttons[0].Find("button").Click();
            //all hideable columns should be hidden;
            switches[0].Instance.Value.Should().BeTrue();
            switches[1].Instance.Value.Should().BeTrue();
            switches[2].Instance.Value.Should().BeTrue();
            switches[3].Instance.Value.Should().BeFalse();
            switches[4].Instance.Value.Should().BeFalse();
            switches[5].Instance.Value.Should().BeFalse();

            // 6 columns, 3 hidden
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(3);

            // this is the show all button
            buttons[1].Find("button").Click();
            switches[0].Instance.Value.Should().BeFalse();
            switches[1].Instance.Value.Should().BeFalse();
            switches[2].Instance.Value.Should().BeFalse();
            switches[3].Instance.Value.Should().BeFalse();
            switches[4].Instance.Value.Should().BeFalse();
            switches[5].Instance.Value.Should().BeFalse();

            // 6 columns, 0 hidden
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(6);

            //programatically changing the hidden which overrides hideable
            await dataGrid.InvokeAsync(async () =>
            {
                foreach (var column in dataGrid.Instance.RenderedColumns)
                {
                    await column.HiddenState.SetValueAsync(true);
                };
            });

            // cannot render the component again there can be only one mudpopoverprovider

            // 6 columns, 6 hidden
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(0);

            //programatically changing the hidden which overrides hideable
            await dataGrid.InvokeAsync(async () =>
            {
                foreach (var column in dataGrid.Instance.RenderedColumns)
                {
                    await column.HiddenState.SetValueAsync(false);
                };
            });

            // 6 columns, 0 hidden
            dataGrid.FindAll(".mud-table-head th").Count.Should().Be(6);
        }

        // This is not easily convertable to the new property expression.
        //[Test]
        //public async Task DataGridFilterRowHiddenTest()
        //{
        //    var comp = Context.RenderComponent<DataGridFilterRowHiddenTest>();
        //    var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterRowHiddenTest.Model>>();

        //    //there should be only one filter cell visible
        //    dataGrid.FindAll(".mud-input-control-input-container").Count.Should().Be(1);

        //    var popoverProvider = comp.FindComponent<MudPopoverProvider>();
        //    var popover = dataGrid.FindComponent<MudPopover>();
        //    popover.Instance.Open.Should().BeFalse("Should start as closed");

        //    var columnsButton = dataGrid.Find("button.mud-button-root.mud-icon-button.mud-ripple.mud-ripple-icon.mud-icon-button-size-small");
        //    columnsButton.Click();

        //    popover.Instance.Open.Should().BeTrue("Should be open once clicked");
        //    var listItems = popoverProvider.FindComponents<MudListItem>();
        //    listItems.Count.Should().Be(1);
        //    var clickablePopover = listItems[0].Find(".mud-list-item");
        //    clickablePopover.Click();

        //    // at this point, the column picker should be open
        //    var switches = dataGrid.FindComponents<MudSwitch<bool>>();
        //    switches.Count.Should().Be(2);

        //    switches[0].Instance.Checked.Should().BeFalse();
        //    switches[1].Instance.Checked.Should().BeTrue();

        //    var buttons = dataGrid.FindComponents<MudButton>();
        //    // this is the hide all button
        //    buttons[0].Find("button").Click();
        //    switches[0].Instance.Checked.Should().BeTrue();
        //    switches[1].Instance.Checked.Should().BeTrue();
        //    // 2 columns, 2 hidden
        //    dataGrid.FindAll(".mud-input-control-input-container").Count.Should().Be(0);

        //    // this is the show all button
        //    buttons[1].Find("button").Click();
        //    switches[0].Instance.Checked.Should().BeFalse();
        //    switches[1].Instance.Checked.Should().BeFalse();
        //    // 2 columns, 0 hidden
        //    dataGrid.FindAll(".mud-input-control-input-container").Count.Should().Be(2);
        //    dataGrid.Instance.RenderedColumns[0].Filterable = false;
        //    await comp.InvokeAsync(dataGrid.Instance.ExternalStateHasChanged);
        //    //If the column is visible and Filterable is false there still shouldďbe the cell
        //    //without the input
        //    dataGrid.FindAll(".mud-table-cell.filter-header-cell").Count.Should().Be(2);
        //    dataGrid.FindAll(".mud-input-control-input-container").Count.Should().Be(1);
        //}

        [Test]
        public void DataGridShowMenuIconTest()
        {
            var comp = Context.RenderComponent<DataGridShowMenuIconTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridShowMenuIconTest.Item>>();
            dataGrid.FindAll(".mud-table-toolbar .mud-menu").Should().BeEmpty();
            var parameters = new List<ComponentParameter>();
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.ShowMenuIcon), true));
            dataGrid.SetParametersAndRender(parameters.ToArray());
            dataGrid.FindAll(".mud-table-toolbar .mud-menu").Should().NotBeEmpty();
        }

        [Test]
        public async Task DataGridColumnPopupFilteringTest()
        {
            var comp = Context.RenderComponent<DataGridColumnPopupFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnPopupFilteringTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            comp.Find(".filter-button").Click();
            var input = comp.FindComponent<MudTextField<string>>();
            var parameters = new List<ComponentParameter>();
            parameters.Add(ComponentParameter.CreateParameter(nameof(input.Instance.Value), "test"));
            input.SetParametersAndRender(parameters.ToArray());
            comp.Find(".apply-filter-button").Click();

            await comp.InvokeAsync(() =>
            {
                return dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridColumnPopupFilteringTest.Model>
                {
                    Column = dataGrid.Instance.RenderedColumns.First(),
                    Operator = FilterOperator.String.Contains,
                    Value = "test"
                });
            });

            dataGrid.Render();
            dataGrid.FindAll("tbody tr").Count.Should().Be(0);
        }

        [Test]
        public async Task DataGridColumnPopupFilteringEmptyTest()
        {
            var comp = Context.RenderComponent<DataGridColumnPopupFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnPopupFilteringTest.Model>>();

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilter());
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);

            await comp.InvokeAsync(() => dataGrid.Instance.CloseFilters());
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0);
        }

        [Test]
        public async Task DataGridColumnPopupFilteringIntentionalEmptyTest()
        {
            var comp = Context.RenderComponent<DataGridColumnPopupFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnPopupFilteringTest.Model>>();

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilter());
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions[0].Operator = FilterOperator.String.Empty;

            await comp.InvokeAsync(() => dataGrid.Instance.CloseFilters());
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
        }

        [Test]
        public void DataGridFilterableFalseTest()
        {
            var comp = Context.RenderComponent<DataGridFilterableFalseTest>();

            comp.Find(".filter-button").Click();
            comp.FindAll(".filters-panel").Count.Should().Be(1);

            comp.FindAll("div.mud-input-control")[0].PointerDown();
            comp.FindAll("div.mud-list-item").Count.Should().Be(3);
        }

        [Test]
        public async Task DataGridColumnPopupCustomFilteringTest()
        {
            var comp = Context.RenderComponent<DataGridColumnPopupCustomFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnPopupCustomFilteringTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            comp.Instance.FilterHired = true;
            await comp.InvokeAsync(async () =>
            {
                var filterContext = dataGrid.Instance.RenderedColumns[3].FilterContext;
                await comp.Instance.ApplyFilterAsync(filterContext);
            });

            dataGrid.FindAll("tbody tr").Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridCustomFilteringTest()
        {
            var comp = Context.RenderComponent<DataGridCustomFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCustomFilteringTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            await comp.InvokeAsync(async () =>
            {
                await comp.Instance.FilterHiredToggled(true, dataGrid.Instance);
            });

            dataGrid.Render();
            dataGrid.FindAll("tbody tr").Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridCustomPropertyFilterTemplateTest()
        {
            var comp = Context.RenderComponent<DataGridCustomPropertyFilterTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCustomPropertyFilterTemplateTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);

            comp.Find(".filter-button").Click();
            var input = comp.FindComponent<MudTextField<string>>();
            await comp.InvokeAsync(async () => await input.Instance.ValueChanged.InvokeAsync("Ira"));
            comp.Find(".apply-filter-button").Click();

            dataGrid.FindAll("tbody tr").Count.Should().Be(1);

            comp.Find(".filter-button").Click();
            comp.Find(".reset-filter-button").Click();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);
        }

        [Test]
        public async Task DataGridCustomPropertyFilterTemplateApplyFilterTwiceTest()
        {
            var comp = Context.RenderComponent<DataGridCustomPropertyFilterTemplateTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCustomPropertyFilterTemplateTest.Model>>();

            dataGrid.FindAll("tbody tr").Count.Should().Be(4);
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0);

            // Apply the filter the first time
            comp.Find(".filter-button").Click();
            var input = comp.FindComponent<MudTextField<string>>();
            await comp.InvokeAsync(async () => await input.Instance.ValueChanged.InvokeAsync("Ira"));
            comp.Find(".apply-filter-button").Click();

            dataGrid.FindAll("tbody tr").Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);

            // Apply the filter a second time
            comp.Find(".filter-button").Click();
            comp.Find(".apply-filter-button").Click();

            dataGrid.FindAll("tbody tr").Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
        }

        [Test]
        public void DataGridShowFilterIconTest()
        {
            var comp = Context.RenderComponent<DataGridCustomFilteringTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCustomFilteringTest.Model>>();
            var parameters = new List<ComponentParameter>();
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.Filterable), false));
            dataGrid.SetParametersAndRender(parameters.ToArray());
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll(".filter-button").Should().BeEmpty();
            parameters.Clear();
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.Filterable), true));
            dataGrid.SetParametersAndRender(parameters.ToArray());
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll(".filter-button").Should().NotBeEmpty();
            parameters.Clear();
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.ShowFilterIcons), false));
            dataGrid.SetParametersAndRender(parameters.ToArray());
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll(".filter-button").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridServerDataColumnFilterMenuTest()
        {
            var comp = Context.RenderComponent<DataGridServerDataColumnFilterMenuTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerDataColumnFilterMenuTest.Model>>();
            var callCountText = comp.FindComponent<MudText>();
            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4);
            callCountText.Markup.Should().Contain("Server call count: 1");

            comp.Find(".filter-button").Click();
            var input = comp.FindComponent<MudTextField<string>>();
            await comp.InvokeAsync(async () => await input.Instance.ValueChanged.InvokeAsync("Sam"));
            comp.Find(".apply-filter-button").Click();
            callCountText.Markup.Should().Contain("Server call count: 2");
            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(1);

            comp.Find(".filter-button").Click();
            comp.Find(".clear-filter-button").Click();
            callCountText.Markup.Should().Contain("Server call count: 3");
            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4);
        }

        [Test]
        public async Task DataGridServerDataColumnFilterMenuApplyTwiceTest()
        {
            var comp = Context.RenderComponent<DataGridServerDataColumnFilterMenuTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerDataColumnFilterMenuTest.Model>>();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4);
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(0);

            // Apply the filter the first time
            comp.Find(".filter-button").Click();
            var input = comp.FindComponent<MudTextField<string>>();
            await comp.InvokeAsync(async () => await input.Instance.ValueChanged.InvokeAsync("Sam"));
            comp.Find(".apply-filter-button").Click();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);

            // Apply the filter a second time
            comp.Find(".filter-button").Click();
            comp.Find(".apply-filter-button").Click();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
        }

        [Test]
        public async Task DataGridServerDataColumnFilterRowTest()
        {
            var comp = Context.RenderComponent<DataGridServerDataColumnFilterRowTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridServerDataColumnFilterRowTest.Model>>();
            var callCountText = comp.FindComponent<MudText>();
            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4);
            callCountText.Markup.Should().Contain("Server call count: 1");

            var input = comp.FindComponent<MudTextField<string>>();
            await comp.InvokeAsync(async () => await input.Instance.ValueChanged.InvokeAsync("Sam"));
            callCountText.Markup.Should().Contain("Server call count: 2");
            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(1);

            comp.Find("th > div > button.mud-button-root").Click(); // Clear filter button
            callCountText.Markup.Should().Contain("Server call count: 3");
            dataGrid.Render();
            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(4);
        }

        [Test]
        public void DataGridColumnFilterRowPropertyTest()
        {
            var comp = Context.RenderComponent<DataGridColumnFilterRowPropertyTest>();

            Assert.DoesNotThrow(() => comp.FindComponent<MudTextField<string>>());
            Assert.DoesNotThrow(() => comp.FindComponent<MudNumericField<double?>>());
            Assert.DoesNotThrow(() => comp.FindComponent<MudSelect<Enum>>());
            Assert.DoesNotThrow(() => comp.FindComponent<MudSelect<bool?>>());
            Assert.DoesNotThrow(() => comp.FindComponent<MudDatePicker>());
        }

        [Test]
        public void DataGridColumnFilterRowPropertyClearTest()
        {
            var comp = Context.RenderComponent<DataGridColumnFilterRowPropertyTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnFilterRowPropertyTest.Model>>();

            var inputsBefore = dataGrid.FindAll("input").OfType<IHtmlInputElement>().Select(e => e.Value).ToList();
            var hireDate = new DateTime(2011, 1, 2).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture);
            inputsBefore.Should().BeEquivalentTo("Ira", "27", "Success", "True", hireDate, "00:00");

            IRefreshableElementCollection<IElement> ClearButtons() => dataGrid.FindAll(".align-self-center");
            ClearButtons().Should().HaveCount(5);
            ClearAllFiltersOneByOne();

            var inputsAfter = dataGrid.FindAll("input").OfType<IHtmlInputElement>().Select(e => e.Value).ToList();
            inputsAfter.Should().HaveCount(6).And.AllBe("", because: "clicking the clear buttons should reset all filters");

            var action = ClearAllFiltersOneByOne;

            // We had regressions here before https://github.com/MudBlazor/MudBlazor/issues/10034
            action.Should().NotThrow("We click clear again to make sure that no exception appear when there are no filters left.");

            void ClearAllFiltersOneByOne()
            {
                for (var index = 0; index < ClearButtons().Count; index++)
                {
                    var clearButton = ClearButtons()[index];
                    clearButton.Click();
                }
            }
        }

        [Test]
        public void DataGridColumnFilterRowPropertyClearAllTest()
        {
            var comp = Context.RenderComponent<DataGridColumnFilterRowPropertyTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnFilterRowPropertyTest.Model>>();

            var inputsBefore = dataGrid.FindAll("input").OfType<IHtmlInputElement>().Select(e => e.Value).ToList();
            var hireDate = new DateTime(2011, 1, 2).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern, CultureInfo.CurrentCulture);
            inputsBefore.Should().BeEquivalentTo("Ira", "27", "Success", "True", hireDate, "00:00");

            dataGrid.Find(".clear-all-filters").Click();

            var inputsAfter = dataGrid.FindAll("input").OfType<IHtmlInputElement>().Select(e => e.Value).ToList();
            inputsAfter.Should().HaveCount(6).And.AllBe("", because: "clicking the clear button should reset all filters");
        }

        [Test]
        public void DataGridStickyColumnsTest()
        {
            var comp = Context.RenderComponent<DataGridStickyColumnsTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridStickyColumnsTest.Model>>();

            dataGrid.Find("th").ClassList.Should().Contain("sticky-left");
            dataGrid.FindAll("th").Last().ClassList.Should().Contain("sticky-right");
        }

        [Test]
        public void DataGridStickyColumnsResizerTest()
        {
            var comp = Context.RenderComponent<DataGridStickyColumnsResizerTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridStickyColumnsResizerTest.Model>>();

            var header = dataGrid.Find(".mud-table-toolbar");
            header.GetAttribute("style").Should().Contain("position:sticky");
            header.GetAttribute("style").Should().Contain("left:0px");

            var footer = dataGrid.Find(".mud-table-pagination");
            footer.GetAttribute("style").Should().Contain("position:sticky");
            footer.GetAttribute("style").Should().Contain("left:0px");

            var body = dataGrid.Find(".mud-table-container");
            body.GetAttribute("style").Should().Contain("width:max-content");
            body.GetAttribute("style").Should().Contain("overflow:clip");

            dataGrid.Find("th").ClassList.Should().Contain("sticky-left");
            dataGrid.FindAll("th").Last().ClassList.Should().Contain("sticky-right");
        }

        [Test]
        public async Task DataGridCellContextTest()
        {
            var comp = Context.RenderComponent<DataGridCellContextTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCellContextTest.Model>>();

            var item = dataGrid.Instance.Items.FirstOrDefault();

            var column = dataGrid.Instance.RenderedColumns.First();
            var cell = new Cell<DataGridCellContextTest.Model>(dataGrid.Instance, column, item);

            cell._cellContext.Selected.Should().Be(false);
            await cell._cellContext.Actions.SetSelectedItemAsync(true);
            cell._cellContext.Selected.Should().Be(true);

            await cell._cellContext.Actions.ToggleHierarchyVisibilityForItemAsync();
            cell._cellContext.OpenHierarchies.Should().Contain(item);
            cell._cellContext.Open.Should().Be(true);
            await cell._cellContext.Actions.ToggleHierarchyVisibilityForItemAsync();
            cell._cellContext.OpenHierarchies.Should().NotContain(item);
            cell._cellContext.Open.Should().Be(false);
        }

        [Test]
        public void DataGridAggregationTest()
        {
            var comp = Context.RenderComponent<DataGridAggregationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridAggregationTest.Model>>();

            dataGrid.FindAll("td.footer-cell")[1].TrimmedText().Should().Be("Average age is 56");
            dataGrid.FindAll("tfoot td.footer-cell")[1].TrimmedText().Should().Be("Average age is 43");
        }

        [Test]
        public void DataGridSequenceContainsNoElementsTest()
        {
            // Arrange & Act
            var component = Context.RenderComponent<DataGridSequenceContainsNoElementsTest>();
            var dataGridComponent = () => component.FindComponent<MudDataGrid<DataGridSequenceContainsNoElementsTest.Model>>();

            // This test will result in an error if the 'sequence contains no elements' issue is present.
            // Assert
            dataGridComponent.Should().NotThrow();
        }

        [Test]
        public void DataGridObservabilityTest()
        {
            var comp = Context.RenderComponent<DataGridObservabilityTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridObservabilityTest.Model>>();

            var addButton = comp.Find(".add-item-btn");
            var removeButton = comp.Find(".remove-item-btn");

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(8);

            addButton.Click();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(9);

            removeButton.Click();

            dataGrid.FindAll(".mud-table-body .mud-table-row").Count.Should().Be(8);
        }

        public void TableFilterGuid()
        {
            var comp = Context.RenderComponent<DataGridFilterGuid<Guid>>();
            var grid = comp.Instance.MudGridRef;

            grid.Items.Count().Should().Be(2);
            grid.FilteredItems.Count().Should().Be(2);
            var guidColumn = grid.RenderedColumns.FirstOrDefault(x => x.PropertyName == "Id");

            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid>.WeatherForecast>()
            {
                Column = guidColumn,
                Operator = "equals",
                Value = "invalid guid",
            });
            grid.FilteredItems.Count().Should().Be(0);

            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid>.WeatherForecast>()
            {
                Column = guidColumn,
                Operator = "equals",
                Value = comp.Instance.Guid1,
            });
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()?.Id.Should().Be(comp.Instance.Guid1);

            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid>.WeatherForecast>()
            {
                Column = guidColumn,
                Operator = "not equals",
                Value = comp.Instance.Guid1,
            });
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()?.Id.Should().Be(comp.Instance.Guid2);
        }

        [Test]
        public void TableFilterNullableGuid()
        {
            var comp = Context.RenderComponent<DataGridFilterGuid<Guid?>>();
            var grid = comp.Instance.MudGridRef;

            grid.Items.Count().Should().Be(2);
            grid.FilteredItems.Count().Should().Be(2);
            var guidColumn = grid.RenderedColumns.FirstOrDefault(x => x.PropertyName == "Id");

            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid?>.WeatherForecast>()
            {
                Column = guidColumn,
                Operator = "equals",
                //Value = "invalid guid", cannot be a string here...
                Value = Guid.Empty
            });
            comp.Render();
            grid.FilteredItems.Count().Should().Be(0);

            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid?>.WeatherForecast>()
            {
                Column = guidColumn,
                Operator = "equals",
                Value = comp.Instance.Guid1,
            });
            comp.Render();
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()?.Id.Should().Be(comp.Instance.Guid1);

            grid.FilterDefinitions.Clear();
            grid.FilterDefinitions.Add(new FilterDefinition<DataGridFilterGuid<Guid?>.WeatherForecast>()
            {
                Column = guidColumn,
                Operator = "not equals",
                Value = comp.Instance.Guid1,
            });
            comp.Render();
            grid.FilteredItems.Count().Should().Be(1);
            grid.FilteredItems.FirstOrDefault()?.Id.Should().Be(comp.Instance.Guid2);
        }

        //[Test]
        //public async Task TableFilterGuidInDictionary()
        //{
        //    var comp = Context.RenderComponent<DataGridFilterDictionaryGuid>();
        //    var grid = comp.Instance.MudGridRef;

        //    grid.Items.Count().Should().Be(2);
        //    grid.FilteredItems.Count().Should().Be(2);
        //    var guidColumn = grid.RenderedColumns.FirstOrDefault(x => x.PropertyName == "Id");

        //    grid.FilterDefinitions.Add(new FilterDefinition<IDictionary<string, object>>()
        //    {
        //        Column = guidColumn,
        //        Operator = "equals",
        //        Value = "invalid guid",
        //    });
        //    grid.FilteredItems.Count().Should().Be(0);

        //    grid.FilterDefinitions.Clear();
        //    grid.FilterDefinitions.Add(new FilterDefinition<IDictionary<string, object>>()
        //    {
        //        Column = guidColumn,
        //        Operator = "equals",
        //        Value = comp.Instance.Guid1,
        //    });
        //    grid.FilteredItems.Count().Should().Be(1);
        //    grid.FilteredItems.FirstOrDefault()["Id"].Should().Be(Guid.Parse(comp.Instance.Guid1));

        //    grid.FilterDefinitions.Clear();
        //    grid.FilterDefinitions.Add(new FilterDefinition<IDictionary<string, object>>()
        //    {
        //        Column = guidColumn,
        //        Operator = "not equals",
        //        Value = comp.Instance.Guid1,
        //    });
        //    grid.FilteredItems.Count().Should().Be(1);
        //    grid.FilteredItems.FirstOrDefault()["Id"].Should().Be(Guid.Parse(comp.Instance.Guid2));
        //}

        [Test]
        public void DataGridCultureColumnSimpleTest()
        {
            var comp = Context.RenderComponent<DataGridCultureSimpleTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCultureSimpleTest.Model>>();

            dataGrid.FindAll("td")[2].TextContent.Trim().Should().Be("3.5");
            dataGrid.FindAll("td")[3].TextContent.Trim().Should().Be("5,2");
        }

        [Test]
        public void DataGridCultureColumnEditableTest()
        {
            var comp = Context.RenderComponent<DataGridCultureEditableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCultureEditableTest.Model>>();

            dataGrid.FindAll("td input")[2].GetAttribute("value").Trim().Should().Be("3.5");
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("5,2");
        }

        [Test]
        public void DataGridCultureColumnFilterTest()
        {
            var comp = Context.RenderComponent<DataGridCultureSimpleTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCultureSimpleTest.Model>>();

            // amount with invariant culture (decimals separated by point)
            var amountHeader = dataGrid.FindAll("th .mud-menu button")[2];
            amountHeader.Click();

            var filterAmount = comp.FindAll(".mud-list-item")[1];
            filterAmount.Click();

            var filterField = comp.Find(".filters-panel .filter-field .mud-select-input");
            filterField.TextContent.Trim().Should().Be("Amount");

            var filterInput = comp.FindAll(".filters-panel input")[2];
            filterInput.Input(new ChangeEventArgs() { Value = "2,2" });

            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions[0].Value.Should().Be(22.0);

            dataGrid.Instance.FilterDefinitions.Clear();
            dataGrid.Render();

            // total with es-ES culture (decimals separated by comma)
            var totalHeader = dataGrid.FindAll("th .mud-menu button")[3];
            totalHeader.Click();
            var filterTotal = comp.FindAll(".mud-list-item")[1];
            filterTotal.Click();

            var filterTotalField = comp.Find(".filters-panel .filter-field .mud-select-input");
            filterTotalField.TextContent.Trim().Should().Be("Total");

            var filterTotalInput = comp.FindAll(".filters-panel input")[2];
            filterTotalInput.Input(new ChangeEventArgs() { Value = "2,2" });

            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions[0].Value.Should().Be(2.2);
        }

        [Test]
        public void DataGridCultureColumnFilterHeaderTest()
        {
            var comp = Context.RenderComponent<DataGridCultureEditableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCultureEditableTest.Model>>();

            // amount with invariant culture (decimals separated by point)
            var filterAmount = dataGrid.FindAll("th.filter-header-cell input")[2];
            filterAmount.Input(new ChangeEventArgs() { Value = "2,2" });

            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions[0].Value.Should().Be(22.0);

            dataGrid.Instance.FilterDefinitions.Clear();
            dataGrid.Render();

            // total with es-ES culture (decimals separated by comma)
            var filterTotal = dataGrid.FindAll("th.filter-header-cell input")[3];
            filterTotal.Input(new ChangeEventArgs() { Value = "2,2" });

            dataGrid.Instance.FilterDefinitions.Count.Should().Be(1);
            dataGrid.Instance.FilterDefinitions[0].Value.Should().Be(2.2);
        }

        [Test]
        public void DataGridCultureColumnOverridesTest()
        {
            var comp = Context.RenderComponent<DataGridCulturesTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCulturesTest.Model>>();

            // amount with invariant culture (decimals separated by point)
            dataGrid.FindAll("td input")[2].GetAttribute("value").Trim().Should().Be("3.5");
            // total with 'es' culture (decimals separated by commas)
            dataGrid.FindAll("td input")[3].GetAttribute("value").Trim().Should().Be("5,2");
            // distance with custom culture (decimals separated by '#')
            dataGrid.FindAll("td input")[4].GetAttribute("value").Trim().Should().Be("2#1");
        }

        [Test]
        public async Task DataGridSortIndicatorTest()
        {
            var comp = Context.RenderComponent<DataGridSortableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSortableTest.Item>>();

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Value", SortDirection.Ascending, x => x.Value));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll("th .sortable-column-header")[1].TextContent.Trim().Should().Be("Value");
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.Ascending);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Value", SortDirection.Descending, x => x.Value));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.Descending);
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-desc").Should().Be(true);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => x.Value));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.Ascending);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.None);
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(false);
        }

        [Test]
        public async Task DataGridParentAndChildSamePropertyNameSortTest()
        {
            var comp = Context.RenderComponent<DataGridChildPropertiesWithSameNameSortTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridChildPropertiesWithSameNameSortTest.Employee>>();

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Manager.Name", SortDirection.Ascending, x => x.Manager.Name));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll("th .sortable-column-header")[1].TextContent.Trim().Should().Be("Name");
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            dataGrid.Instance.GetColumnSortDirection("Manager.Name").Should().Be(SortDirection.Ascending);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => x.Name));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll("th .sortable-column-header")[0].TextContent.Trim().Should().Be("Name");
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.Ascending);
            dataGrid.Instance.GetColumnSortDirection("Manager.Name").Should().Be(SortDirection.None);
        }

        [Test]
        public async Task DataGridCustomSortTest()
        {
            var comp = Context.RenderComponent<DataGridCustomSortableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridCustomSortableTest.Item>>();
            dataGrid.Instance.SortMode = SortMode.Single;
            dataGrid.Instance.SortMode.Should().Be(SortMode.Single);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Value", SortDirection.Ascending, x => x.Value, new MudBlazor.Utilities.NaturalComparer()));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll("th .sortable-column-header")[1].TextContent.Trim().Should().Be("Value");
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.Ascending);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Value", SortDirection.Descending, x => x.Value, new MudBlazor.Utilities.NaturalComparer()));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.Descending);
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-desc").Should().Be(true);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Value", SortDirection.Ascending, x => x.Value));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll("th .sortable-column-header")[1].TextContent.Trim().Should().Be("Value");
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(false);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.Ascending);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => x.Name, new MudBlazor.Utilities.NaturalComparer()));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.Ascending);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.None);
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-asc").Should().Be(true);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(false);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Descending, x => x.Name, new MudBlazor.Utilities.NaturalComparer()));
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.Descending);
            dataGrid.Instance.GetColumnSortDirection("Value").Should().Be(SortDirection.None);
            dataGrid.FindAll("th .sort-direction-icon")[0].ClassList.Contains("mud-direction-desc").Should().Be(true);
            dataGrid.FindAll("th .sort-direction-icon")[1].ClassList.Contains("mud-direction-asc").Should().Be(false);

            dataGrid.Instance.SortMode = SortMode.Multiple;
            dataGrid.Instance.SortMode.Should().Be(SortMode.Multiple);

            //Assign a comparer to a column
            var column = dataGrid.FindComponent<Column<DataGridCustomSortableTest.Item>>();
            await comp.InvokeAsync(() => column.Instance.Comparer = new MudBlazor.Utilities.NaturalComparer());
            //Clear sorting
            await comp.InvokeAsync(() => dataGrid.Instance.RemoveSortAsync("Name"));
            dataGrid.Instance.GetColumnSortDirection("Name").Should().Be(SortDirection.None);
            //Sort by clicking on the header cell
            dataGrid.Find(".column-options button").Click();
            var cells = dataGrid.FindAll("td");

            // Check the values of rows - should not be sorted and should be in the original order.
            cells[0].TextContent.Should().Be("0");
            cells[3].TextContent.Should().Be("1");
            cells[6].TextContent.Should().Be("1_2");
            cells[9].TextContent.Should().Be("1_10");
            cells[12].TextContent.Should().Be("1_11");
            cells[15].TextContent.Should().Be("2");
            cells[18].TextContent.Should().Be("10");

            //Multi click second column
            var headerCell = dataGrid.FindComponents<HeaderCell<DataGridCustomSortableTest.Item>>()[1];
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs() { CtrlKey = true, Button = 0 }));
            headerCell.Instance.SortDirection.Should().Be(SortDirection.Ascending);

            //Multi click second column a second time to change it to descending
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs() { CtrlKey = true, Button = 0 }));
            headerCell.Instance.SortDirection.Should().Be(SortDirection.Descending);

            //remove first column from sort
            headerCell = dataGrid.FindComponents<HeaderCell<DataGridCustomSortableTest.Item>>()[0];
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new MouseEventArgs() { AltKey = true, Button = 0 }));
            headerCell.Instance.SortDirection.Should().Be(SortDirection.None);
        }

        [Test]
        public async Task DataGridGroupExpandedTrueTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedTest.Fruit>>();

            comp.FindAll("tbody .mud-table-row").Count.Should().Be(7);
            await comp.InvokeAsync(() => dataGrid.Instance.CollapseAllGroups());
            dataGrid.Render();
            // after all groups are collapsed
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(2);
            await comp.InvokeAsync(() =>
                comp.Instance.AddFruit());
            // datagrid should be expanded with the new category
            dataGrid.Render();
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(5);
        }

        [Test]
        public async Task DataGridGroupExpandedTrueAsyncTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedAsyncTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedAsyncTest.Fruit>>();

            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(7));
            await comp.InvokeAsync(() => dataGrid.Instance.CollapseAllGroups());
            dataGrid.Render();
            // after all groups are collapsed
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(2));
            await comp.InvokeAsync(() =>
                comp.Instance.AddFruit());
            // datagrid should be expanded with the new category
            dataGrid.Render();
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(5));
        }

        [Test]
        public async Task DataGridGroupExpandedTrueServerDataTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedServerDataTest.Fruit>>();

            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(7));
            await comp.InvokeAsync(() => dataGrid.Instance.CollapseAllGroups());
            dataGrid.Render();
            // after all groups are collapsed
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(2));
            await comp.InvokeAsync(() => comp.Instance.AddFruit());
            // datagrid should be expanded with the new category
            dataGrid.Render();
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(5));
        }

        [Test]
        public async Task DataGridGroupExpandedFalseTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedFalseTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedFalseTest.Fruit>>();

            comp.FindAll("tbody .mud-table-row").Count.Should().Be(2);
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroups());
            dataGrid.Render();
            // after all groups are expanded
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(7);
            await comp.InvokeAsync(() =>
                comp.Instance.AddFruit());
            // datagrid should be collapsed with the new category
            dataGrid.Render();
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(8);
        }

        [Test]
        public async Task DataGridGroupExpandedFalseAsyncTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedFalseAsyncTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedFalseAsyncTest.Fruit>>();

            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(2));
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroups());
            dataGrid.Render();
            // after all groups are expanded
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(7));
            await comp.InvokeAsync(() =>
                comp.Instance.AddFruit());
            // datagrid should be collapsed with the new category
            dataGrid.Render();
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(8));
        }

        [Test]
        public async Task DataGridGroupExpandedFalseServerDataTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandedFalseServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandedFalseServerDataTest.Fruit>>();

            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(2));
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroups());
            dataGrid.Render();
            // after all groups are expanded
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(7));
            await comp.InvokeAsync(() => comp.Instance.AddFruit());
            // datagrid should be collapsed with the new category
            dataGrid.Render();
            comp.WaitForAssertion(() => comp.FindAll("tbody .mud-table-row").Count.Should().Be(8));
        }

        [Test]
        public async Task DataGridGroupCollapseAllTest()
        {
            var comp = Context.RenderComponent<DataGridGroupCollapseAllTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupCollapseAllTest.TestObject>>();

            comp.FindAll("tbody .mud-table-row").Count.Should().Be(3);
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroups());
            comp.Render();
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(15);
            await comp.InvokeAsync(() => dataGrid.Instance.CollapseAllGroups());
            comp.Render();
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(3);
            comp.Instance.RefreshList();
            comp.Render();
            // after all groups are expanded
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(3);
        }

        [Test]
        public async Task DataGridGroupExpandAllCollapseAllTest()
        {
            var comp = Context.RenderComponent<DataGridGroupExpandAllCollapseAllTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupExpandAllCollapseAllTest.Element>>();

            comp.FindAll("tbody .mud-table-row").Count.Should().Be(2);
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroups());
            comp.Render();
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(14);
            await dataGrid.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.First));
            await dataGrid.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.Next));
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(18);
            await comp.InvokeAsync(() => dataGrid.Instance.CollapseAllGroups());
            await dataGrid.InvokeAsync(() => dataGrid.Instance.NavigateTo(Page.First));
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(2);
            comp.Instance.RefreshList();
            comp.Render();
            comp.FindAll("tbody .mud-table-row").Count.Should().Be(2);
        }

        [Test]
        public async Task DataGridPropertyColumnFormatTest()
        {
            var comp = Context.RenderComponent<DataGridFormatTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFormatTest.Employee>>();

            comp.FindAll("tbody.mud-table-body td")[3].TextContent.Should().Be("$87,000.00");
            var column = (PropertyColumn<DataGridFormatTest.Employee, int>)dataGrid.Instance.GetColumnByPropertyName("Salary");
            await comp.InvokeAsync(() => column.Format = "C0");
            comp.Find(".mud-switch-input").Change(new ChangeEventArgs { Value = true });
            comp.FindAll("tbody.mud-table-body td")[3].TextContent.Should().Be("$87,000");
        }

        [Test]
        public async Task DataGridFilteredItemsCacheTest()
        {
            var comp = Context.RenderComponent<DataGridSortableTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSortableTest.Item>>();

            var initialFilterCount = dataGrid.Instance.FilteringRunCount;

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Ascending, x => { return x.Name; }));
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 1);

            await comp.InvokeAsync(() => dataGrid.Instance.SetSortAsync("Name", SortDirection.Descending, x => { return x.Name; }));
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 2);

            await comp.InvokeAsync(() => dataGrid.Instance.RemoveSortAsync("Name"));
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 3);


            var column = dataGrid.FindComponent<Column<DataGridSortableTest.Item>>();
            await comp.InvokeAsync(() => column.Instance.SortBy = x => { return x.Name; });
            dataGrid.Render();
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 4);

            // sort through the sort icon
            dataGrid.Find(".column-options button").Click();
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 5);

            // test other sort methods
            var headerCell = dataGrid.FindComponent<HeaderCell<DataGridSortableTest.Item>>();
            await comp.InvokeAsync(() => headerCell.Instance.SortChangedAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs()));
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 6);

            //await comp.InvokeAsync(() => headerCell.Instance.GetDataType());
            await comp.InvokeAsync(() => headerCell.Instance.RemoveSortAsync());
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 7);
            await comp.InvokeAsync(() => headerCell.Instance.AddFilter());
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 8);
            await comp.InvokeAsync(() => headerCell.Instance.OpenFilters());
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 9);

            await comp.InvokeAsync(() => dataGrid.Instance.SortMode = SortMode.None);
            dataGrid.Render();
            dataGrid.Instance.FilteringRunCount.Should().Be(initialFilterCount + 10);
            // Since Sortable is now false, the click handler (and element holding it) should no longer exist.
            dataGrid.Instance.DropContainerHasChanged();
            dataGrid.FindAll(".column-header .sortable-column-header").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridMultiSelectOnRowClickTest()
        {
            var comp = Context.RenderComponent<DataGridMultiSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridMultiSelectionTest.Item>>();

            // click on the first row
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.FindAll("tbody.mud-table-body td")[1].Click();
            dataGrid.Instance.SelectedItems.Count.Should().Be(1);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(1); //ensure selection is rendered

            // click on the second row
            dataGrid.FindAll("tbody.mud-table-body td")[2].Click();
            dataGrid.Instance.SelectedItems.Count.Should().Be(2);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(2);

            var parameters = new List<ComponentParameter>();
            parameters.Add(ComponentParameter.CreateParameter(nameof(dataGrid.Instance.SelectOnRowClick), false));
            dataGrid.SetParametersAndRender(parameters.ToArray());

            // deselect all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(false));
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(0);

            // click on the first row
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.FindAll("tbody.mud-table-body td")[1].Click();
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(0);
        }

        [Test]
        public async Task DataGridSingleSelectOnRowClickTest()
        {
            var comp = Context.RenderComponent<DataGridSingleSelectionTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridSingleSelectionTest.Item>>();

            // click on the first row
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.FindAll("tbody.mud-table-body td")[1].Click();
            dataGrid.Instance.SelectedItems.Count.Should().Be(1);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(1); //ensure selection is rendered

            // click on the second row
            dataGrid.FindAll("tbody.mud-table-body td")[2].Click();
            dataGrid.Instance.SelectedItems.Count.Should().Be(1);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(1);


            // click on the second row
            dataGrid.FindAll("tbody.mud-table-body td")[2].Click();
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(0);

            var parameters = new List<ComponentParameter>
            {
                ComponentParameter.CreateParameter(nameof(dataGrid.Instance.SelectOnRowClick), false)
            };

            dataGrid.SetParametersAndRender(parameters.ToArray());

            // deselect all programmatically
            await comp.InvokeAsync(async () => await dataGrid.Instance.SetSelectAllAsync(false));
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);

            // click on the first row
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.FindAll("tbody.mud-table-body td")[1].Click();
            dataGrid.Instance.SelectedItems.Count.Should().Be(0);
            dataGrid.FindAll(".mud-checkbox-true").Count.Should().Be(0);
        }

        [Test]
        public async Task DataGridDragAndDropTest()
        {
            var comp = Context.RenderComponent<DataGridDragAndDropTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridDragAndDropTest.Model>>();
            dataGrid.Instance.DropContainerHasChanged();

            var headerValues = dataGrid.FindAll(".sortable-column-header");
            headerValues.Count.Should().Be(5, because: "5 columns in DataGridFiltersTest");

            headerValues[0].InnerHtml.Should().Be("Name");
            headerValues[1].InnerHtml.Should().Be("Age");
            headerValues[2].InnerHtml.Should().Be("Status");
            headerValues[3].InnerHtml.Should().Be("Hired");
            headerValues[4].InnerHtml.Should().Be("HiredOn");

            var container = dataGrid.Find(".mud-drop-container");
            container.Children.Should().HaveCount(1);

            var zone = dataGrid.FindAll(".mud-drop-zone");
            zone.Count.Should().Be(5, because: "5 columns in DataGridFiltersTest");

            var firstDropZone = zone[1];
            var firstDropItem = firstDropZone.Children[0];

            var secondDropZone = zone[2];
            var secondDropItem = secondDropZone.Children[0];

            await firstDropItem.DragStartAsync(new DragEventArgs());
            await secondDropItem.DropAsync(new DragEventArgs());

            var newHeaderValues = dataGrid.FindAll(".sortable-column-header");
            newHeaderValues.Count.Should().Be(5, because: "5 columns in DataGridFiltersTest");

            newHeaderValues[0].InnerHtml.Should().Be("Name");
            newHeaderValues[1].InnerHtml.Should().Be("Status");
            newHeaderValues[2].InnerHtml.Should().Be("Age");
            newHeaderValues[3].InnerHtml.Should().Be("Hired");
            newHeaderValues[4].InnerHtml.Should().Be("HiredOn");

        }
        [Test]
        public void DataGridEditFormDialogIsCustomizableTest()
        {
            var comp = Context.RenderComponent<DataGridEditFormCustomizedDialogTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridEditFormCustomizedDialogTest.Model>>();

            //open edit dialog
            dataGrid.FindAll("tbody tr")[1].Click();
            //check if dialog is open
            comp.FindAll("div.mud-dialog-container").Should().NotBeEmpty();
            //find button with arialabel close in dialog
            var closeButton = comp.Find("button[aria-label=\"Close dialog\"]");
            closeButton.Should().NotBeNull();
            //click close button
            comp.Find("button[aria-label=\"Close dialog\"]").Click();
            //check if dialog is closed
            comp.FindAll("div.mud-dialog-container").Should().BeEmpty();
        }

        [Test]
        public async Task DataGridDragAndDropWithDynamicColumnsTest()
        {
            var comp = Context.RenderComponent<DataGridDragAndDropWithDynamicColumnsTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridDragAndDropWithDynamicColumnsTest.Model>>();
            dataGrid.Instance.DropContainerHasChanged();

            var headerValues = dataGrid.FindAll(".sortable-column-header");
            headerValues.Count.Should().Be(5, because: "5 columns in DataGridFiltersTest");

            headerValues[0].InnerHtml.Should().Be("Name");
            headerValues[1].InnerHtml.Should().Be("Age");
            headerValues[2].InnerHtml.Should().Be("Status");
            headerValues[3].InnerHtml.Should().Be("Hired");
            headerValues[4].InnerHtml.Should().Be("HiredOn");

            var container = dataGrid.Find(".mud-drop-container");
            container.Children.Should().HaveCount(1);

            var zone = dataGrid.FindAll(".mud-drop-zone");
            zone.Count.Should().Be(5, because: "5 columns in DataGridFiltersTest");

            var firstDropZone = zone[1];
            var firstDropItem = firstDropZone.Children[0];

            var secondDropZone = zone[2];
            var secondDropItem = secondDropZone.Children[0];

            await firstDropItem.DragStartAsync(new DragEventArgs());
            await secondDropItem.DropAsync(new DragEventArgs());

            var newHeaderValues = dataGrid.FindAll(".sortable-column-header");
            newHeaderValues.Count.Should().Be(5, because: "5 columns in DataGridFiltersTest");

            newHeaderValues[0].InnerHtml.Should().Be("Name");
            newHeaderValues[1].InnerHtml.Should().Be("Status");
            newHeaderValues[2].InnerHtml.Should().Be("Age");
            newHeaderValues[3].InnerHtml.Should().Be("Hired");
            newHeaderValues[4].InnerHtml.Should().Be("HiredOn");
        }

        [Test]
        public void DataGridRedundantMenuTest()
        {
            var comp = Context.RenderComponent<DataGridRedundantMenuTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridRedundantMenuTest.Model>>();

            dataGrid.Instance.FilterMode = DataGridFilterMode.ColumnFilterRow;
            dataGrid.Instance.SortMode = SortMode.None;

            // Render after applying conditions
            comp.Render();

            // Assert that the `column-options` span is present but empty
            var columnOptionsSpan = comp.Find(".column-options");
            columnOptionsSpan.Should().NotBeNull();
            columnOptionsSpan.TextContent.Trim().Should().BeEmpty();
        }

        [Test]
        public void ShouldSetIsGenderGroupedToTrueWhenGroupingIsApplied()
        {
            // Render the DataGridGroupingTest component for testing.
            var comp = Context.RenderComponent<DataGridColumnGroupingTest>();

            // Attempt to find the MudPopoverProvider component within the rendered component.
            // MudPopoverProvider is used to manage popovers in the component, including the grouping popover.
            var popoverProvider = comp.FindComponent<MudPopoverProvider>();

            // Assert that initially, before any user interaction, IsGenderGrouped should be false.
            comp.Instance.IsGenderGrouped.Should().Be(false);

            // Find the button within the 'th' element with class 'gender' that triggers the popover for grouping.
            var genderHeaderOption = comp.Find("th.gender .mud-menu button");

            // Simulate a click on the gender header group button to open the popover with grouping options.
            genderHeaderOption.Click();

            // Find all MudListItem components within the popoverProvider.
            // These list items represent the individual options within the grouping popover.
            var listItems = popoverProvider.FindComponents<MudListItem<object>>();

            // Assert that there are exactly 2 list items (options) available in the popover.
            listItems.Count.Should().Be(2);

            // From the list items found, select the second one which is expected to be the clickable option for grouping.
            var clickablePopover = listItems[1].Find(".mud-list-item");

            // click on the grouping option to apply grouping to the data grid.
            clickablePopover.Click();

            // After clicking the grouping option, assert that IsGenderGrouped is now true, indicating that
            // the action of applying grouping has successfully updated the component's state.
            comp.Instance.IsGenderGrouped.Should().Be(true);
        }

        [Test]
        public void DataGridDynamicColumnsTest()
        {
            var comp = Context.RenderComponent<DataGridDynamicColumnsTest>();

            comp.Instance.GridRenderedColumnsCount.Should().Be(0);

            comp.Instance.AddColumns();

            comp.Instance.GridRenderedColumnsCount.Should().Be(3);

            comp.Instance.RemoveColumn();

            comp.Instance.GridRenderedColumnsCount.Should().Be(2);

            comp.Instance.RemoveAllColumns();

            comp.Instance.GridRenderedColumnsCount.Should().Be(0);
        }

        [Test]
        public void DataGridSelectColumnTest()
        {
            var comp = Context.RenderComponent<DataGridSelectColumnTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<int>>();
            var rowCheckbox = dataGrid.FindAll("td input");
            var selectAllCheckboxes = dataGrid.FindComponents<MudCheckBox<bool?>>();

            selectAllCheckboxes[0].Instance.Value.Should().BeFalse();
            selectAllCheckboxes[1].Instance.Value.Should().BeFalse();

            rowCheckbox[0].Change(true);

            selectAllCheckboxes[0].Instance.Value.Should().Be(default);
            selectAllCheckboxes[1].Instance.Value.Should().Be(default);

            rowCheckbox[1].Change(true);

            selectAllCheckboxes[0].Instance.Value.Should().BeTrue();
            selectAllCheckboxes[1].Instance.Value.Should().BeTrue();

            rowCheckbox[1].Change(false);

            selectAllCheckboxes[0].Instance.Value.Should().Be(default);
            selectAllCheckboxes[1].Instance.Value.Should().Be(default);

            rowCheckbox[0].Change(false);

            selectAllCheckboxes[0].Instance.Value.Should().BeFalse();
            selectAllCheckboxes[1].Instance.Value.Should().BeFalse();
        }

        [Test]
        [SetCulture("en-US")]
        public void DataGridServerGroupUngroupingTest()
        {
            var comp = Context.RenderComponent<DataGridServerDataColumnGroupingTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<Examples.Data.Models.Element>>();
            var popoverProvider = comp.FindComponent<MudPopoverProvider>();

            //click name grouping in grid
            var headerOption = comp.Find("th.name .mud-menu button");
            headerOption.Click();
            var listItems = popoverProvider.FindComponents<MudListItem<object>>();
            listItems.Count.Should().Be(4);
            var clickablePopover = listItems[3].Find(".mud-list-item");
            clickablePopover.Click();
            var cells = dataGrid.FindAll("td");

            //checking cell content is the most reliable way to verify grouping
            cells[0].TextContent.Should().Be("Name: Hydrogen");
            cells[1].TextContent.Should().Be("Name: Helium");
            cells[2].TextContent.Should().Be("Name: Lithium");
            cells[3].TextContent.Should().Be("Name: Beryllium");
            cells[4].TextContent.Should().Be("Name: Boron");
            cells[5].TextContent.Should().Be("Name: Carbon");
            cells[6].TextContent.Should().Be("Name: Nitrogen");
            cells[7].TextContent.Should().Be("Name: Oxygen");
            cells[8].TextContent.Should().Be("Name: Fluorine");
            cells[9].TextContent.Should().Be("Name: Neon");
            dataGrid.Instance.GroupedColumn.Should().NotBeNull();

            //click name ungrouping in grid
            headerOption = comp.Find("th.name .mud-menu button");
            headerOption.Click();
            listItems = popoverProvider.FindComponents<MudListItem<object>>();
            listItems.Count.Should().Be(4);
            clickablePopover = listItems[3].Find(".mud-list-item");
            clickablePopover.Click();
            cells = dataGrid.FindAll("td");
            // We do not need check all 10 rows as it's clear that it's ungrouped if first row pass
            cells[0].TextContent.Should().Be("1");
            cells[1].TextContent.Should().Be("H");
            cells[2].TextContent.Should().Be("Hydrogen");
            cells[3].TextContent.Should().Be("0");
            cells[4].TextContent.Should().Be("1.00794");
            cells[5].TextContent.Should().Be("Other");
            dataGrid.Instance.GroupedColumn.Should().BeNull();
        }

        [Test]
        public void DataGridGroupingTestBoundAndUnboundScenarios()
        {
            var comp = Context.RenderComponent<DataGridColumnGroupingTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridColumnGroupingTest.Model>>();
            var popoverProvider = comp.FindComponent<MudPopoverProvider>();

            IRefreshableElementCollection<IElement> Rows() => dataGrid.FindAll("tr");
            IRefreshableElementCollection<IElement> Cells() => dataGrid.FindAll("td");

            // Assert that initially, before any user interaction, IsGenderGrouped and IsAgeGrouped should be false
            // The default grouping is by name
            comp.Instance.IsGenderGrouped.Should().Be(false);
            comp.Instance.IsAgeGrouped.Should().Be(false);
            comp.Instance.IsProfessionGrouped.Should().Be(false);

            var nameCells = Cells();
            nameCells.Count.Should().Be(4, because: "4 data rows");
            nameCells[0].TextContent.Should().Be("Name: John");
            nameCells[1].TextContent.Should().Be("Name: Johanna");
            nameCells[2].TextContent.Should().Be("Name: Steve");
            nameCells[3].TextContent.Should().Be("Name: Alice");
            Rows().Count.Should().Be(6, because: "1 header row + 4 data rows + 1 footer row");

            var ageGrouping = comp.Find(".GroupByAge");
            ageGrouping.Click();
            comp.Instance.IsAgeGrouped.Should().Be(true);
            comp.Instance.IsGenderGrouped.Should().Be(false);
            comp.Instance.IsProfessionGrouped.Should().Be(false);
            var ageCells = Cells();
            ageCells.Count.Should().Be(3, because: "3 data rows");
            ageCells[0].TextContent.Should().Be("Age: 45");
            ageCells[1].TextContent.Should().Be("Age: 23");
            ageCells[2].TextContent.Should().Be("Age: 32");
            Rows().Count.Should().Be(5, because: "1 header row + 3 data rows + 1 footer row");

            var genderGrouping = comp.Find(".GroupByGender");
            genderGrouping.Click();
            comp.Instance.IsGenderGrouped.Should().Be(true);
            comp.Instance.IsAgeGrouped.Should().Be(true, because: "Age is not bound");
            comp.Instance.IsProfessionGrouped.Should().Be(false);
            var genderCells = Cells();
            genderCells.Count.Should().Be(2, because: "2 data rows");
            genderCells[0].TextContent.Should().Be("Gender: Male");
            genderCells[1].TextContent.Should().Be("Gender: Female");
            Rows().Count.Should().Be(4, because: "1 header row + 2 data rows + 1 footer row");

            var professionGrouping = comp.Find(".GroupByProfession");
            professionGrouping.Click();
            comp.Instance.IsGenderGrouped.Should().Be(false);
            comp.Instance.IsAgeGrouped.Should().Be(true, because: "Age is not bound");
            comp.Instance.IsProfessionGrouped.Should().Be(true);
            var professionCells = Cells();
            professionCells.Count.Should().Be(2, because: "2 data rows");
            professionCells[0].TextContent.Should().Be("Profession: Cook");
            professionCells[1].TextContent.Should().Be("Profession: (None)");
            Rows().Count.Should().Be(4, because: "1 header row + 2 data rows + 1 footer row");

            //click age grouping in grid
            var headerOption = comp.Find("th.age .mud-menu button");
            headerOption.Click();
            var listItems = popoverProvider.FindComponents<MudListItem<object>>();
            listItems.Count.Should().Be(2);
            var clickablePopover = listItems[1].Find(".mud-list-item");
            clickablePopover.Click();
            comp.Instance.IsAgeGrouped.Should().Be(true);
            comp.Instance.IsGenderGrouped.Should().Be(false);
            comp.Instance.IsProfessionGrouped.Should().Be(false);
            Rows().Count.Should().Be(5, because: "1 header row + 3 data rows + 1 footer row");

            //click gender grouping in grid
            headerOption = comp.Find("th.gender .mud-menu button");
            headerOption.Click();
            listItems = popoverProvider.FindComponents<MudListItem<object>>();
            listItems.Count.Should().Be(2);
            clickablePopover = listItems[1].Find(".mud-list-item");
            clickablePopover.Click();
            comp.Instance.IsGenderGrouped.Should().Be(true);
            comp.Instance.IsAgeGrouped.Should().Be(true, because: "Age is not bound");
            comp.Instance.IsProfessionGrouped.Should().Be(false);
            Rows().Count.Should().Be(4, because: "1 header row + 2 data rows + 1 footer row");

            //click Name grouping in grid
            headerOption = comp.Find("th.name .mud-menu button");
            headerOption.Click();
            listItems = popoverProvider.FindComponents<MudListItem<object>>();
            listItems.Count.Should().Be(2);
            clickablePopover = listItems[1].Find(".mud-list-item");
            clickablePopover.Click();
            comp.Instance.IsGenderGrouped.Should().Be(false);
            comp.Instance.IsAgeGrouped.Should().Be(true, because: "Age is not bound");
            comp.Instance.IsProfessionGrouped.Should().Be(false);
            Rows().Count.Should().Be(6, because: "1 header row + 4 data rows + 1 footer row");

            //click profession grouping in grid
            headerOption = comp.Find("th.profession .mud-menu button");
            headerOption.Click();
            listItems = popoverProvider.FindComponents<MudListItem<object>>();
            listItems.Count.Should().Be(2);
            clickablePopover = listItems[1].Find(".mud-list-item");
            clickablePopover.Click();
            comp.Instance.IsGenderGrouped.Should().Be(false);
            comp.Instance.IsAgeGrouped.Should().Be(true, because: "Age is not bound");
            comp.Instance.IsProfessionGrouped.Should().Be(true);
            Rows().Count.Should().Be(4, because: "1 header row + 2 data rows + 1 footer row");
        }

        [Test]
        public async Task FilterDefinitionTestHasFilterProperty()
        {
            var comp = Context.RenderComponent<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFiltersTest.Model>
            { Column = dataGrid.Instance.GetColumnByPropertyName("Name"), Operator = FilterOperator.String.Empty }));

            await comp.InvokeAsync(() => dataGrid.Instance.AddFilterAsync(new FilterDefinition<DataGridFiltersTest.Model>
            { Column = dataGrid.Instance.GetColumnByPropertyName("Age"), Operator = FilterOperator.Number.GreaterThan, Value = 30 }));

            // test filter definition without value
            var nameHeaderCell = dataGrid.FindComponents<HeaderCell<DataGridFiltersTest.Model>>()[0];
            nameHeaderCell.Instance.hasFilter.Should().BeTrue();

            // test filter definition with value
            var ageHeaderCell = dataGrid.FindComponents<HeaderCell<DataGridFiltersTest.Model>>()[1];
            ageHeaderCell.Instance.hasFilter.Should().BeTrue();

            // test filter not applied
            var statusHeaderCell = dataGrid.FindComponents<HeaderCell<DataGridFiltersTest.Model>>()[2];
            statusHeaderCell.Instance.hasFilter.Should().BeFalse();
        }

        [Test]
        public async Task DataGridGroupedWithServerDataPaginationTest()
        {
            var comp = Context.RenderComponent<DataGridGroupableServerDataTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridGroupableServerDataTest.Item>>();
            var rows = dataGrid.FindAll("tr");
            rows.Count.Should().Be(12, because: "1 header row + 10 data rows + 1 footer row");
            var cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(10, because: "We have 10 data rows with one group collapsed");
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroups());
            rows = dataGrid.FindAll("tr");
            rows.Count.Should().Be(32, because: "1 header row + 10 data rows + 1 footer row + 10 group rows + 10 footer group rows");
            cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(30, because: "We have 10 data rows with one group + 10*2 cells inside groups");
            //check cells
            cells[0].TextContent.Should().Be("Number: 1");
            cells[1].TextContent.Should().Be("Hydrogen"); cells[2].TextContent.Should().Be("1");
            cells[3].TextContent.Should().Be("Number: 2");
            cells[4].TextContent.Should().Be("Helium"); cells[5].TextContent.Should().Be("2");
            cells[6].TextContent.Should().Be("Number: 3");
            cells[7].TextContent.Should().Be("Lithium"); cells[8].TextContent.Should().Be("3");
            cells[9].TextContent.Should().Be("Number: 4");
            cells[10].TextContent.Should().Be("Beryllium"); cells[11].TextContent.Should().Be("4");
            cells[12].TextContent.Should().Be("Number: 5");
            cells[13].TextContent.Should().Be("Boron"); cells[14].TextContent.Should().Be("5");
            cells[15].TextContent.Should().Be("Number: 6");
            cells[16].TextContent.Should().Be("Carbon"); cells[17].TextContent.Should().Be("6");
            cells[18].TextContent.Should().Be("Number: 7");
            cells[19].TextContent.Should().Be("Nitrogen"); cells[20].TextContent.Should().Be("7");
            cells[21].TextContent.Should().Be("Number: 8");
            cells[22].TextContent.Should().Be("Oxygen"); cells[23].TextContent.Should().Be("8");
            cells[24].TextContent.Should().Be("Number: 9");
            cells[25].TextContent.Should().Be("Fluorine"); cells[26].TextContent.Should().Be("9");
            cells[27].TextContent.Should().Be("Number: 10");
            cells[28].TextContent.Should().Be("Neon"); cells[29].TextContent.Should().Be("10");
            //get next page
            dataGrid.Instance.CurrentPage = 1;
            await comp.InvokeAsync(async () => await dataGrid.Instance.ReloadServerData());
            cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(10, because: "We have 10 data rows with one group collapsed from next page");
            await comp.InvokeAsync(() => dataGrid.Instance.ExpandAllGroups());
            cells = dataGrid.FindAll("td");
            cells.Count.Should().Be(30, because: "We have next 10 data rows with one group + 10*2 cells inside groups");
            //cells should have data from next page
            cells[0].TextContent.Should().Be("Number: 11");
            cells[1].TextContent.Should().Be("Sodium"); cells[2].TextContent.Should().Be("11");
            cells[3].TextContent.Should().Be("Number: 12");
            cells[4].TextContent.Should().Be("Magnesium"); cells[5].TextContent.Should().Be("12");
            cells[6].TextContent.Should().Be("Number: 13");
            cells[7].TextContent.Should().Be("Aluminium"); cells[8].TextContent.Should().Be("13");
            cells[9].TextContent.Should().Be("Number: 14");
            cells[10].TextContent.Should().Be("Silicon"); cells[11].TextContent.Should().Be("14");
            cells[12].TextContent.Should().Be("Number: 15");
            cells[13].TextContent.Should().Be("Phosphorus"); cells[14].TextContent.Should().Be("15");
            cells[15].TextContent.Should().Be("Number: 16");
            cells[16].TextContent.Should().Be("Sulfur"); cells[17].TextContent.Should().Be("16");
            cells[18].TextContent.Should().Be("Number: 17");
            cells[19].TextContent.Should().Be("Chlorine"); cells[20].TextContent.Should().Be("17");
            cells[21].TextContent.Should().Be("Number: 18");
            cells[22].TextContent.Should().Be("Argon"); cells[23].TextContent.Should().Be("18");
            cells[24].TextContent.Should().Be("Number: 19");
            cells[25].TextContent.Should().Be("Potassium"); cells[26].TextContent.Should().Be("19");
            cells[27].TextContent.Should().Be("Number: 20");
            cells[28].TextContent.Should().Be("Calcium"); cells[29].TextContent.Should().Be("20");
        }

        /// <summary>
        /// Reproduce the bug from https://github.com/MudBlazor/MudBlazor/issues/9585
        /// When a column is hidden by the menu and the precedent column is resized, then the app crash
        /// </summary>
        [Test]
        public async Task DataGrid_ResizeColumn_WhenNeighboringColumnIsHidden()
        {
            // Arrange

            var comp = Context.RenderComponent<DataGridHideAndResizeTest>();
            var dgComp = comp.FindComponent<MudDataGrid<DataGridHideAndResizeTest.Model>>();

            // Act : Hide the middle column and resize the first column

            // Open column the second column header menu
            var columnMenu = comp.FindAll("th .mud-menu button").ElementAt(1);
            columnMenu.Click();

            // Click on the menu item 'Hide'
            comp.WaitForAssertion(() => comp.FindAll(".mud-list-item").ElementAt(1));
            var hideMenuItem = comp.FindAll(".mud-list-item").ElementAt(1);
            hideMenuItem.Click();

            // Mock mudElementRef.getBoundingClientRect for DataGrid and visible columns
            var gridElement = (ElementReference)dgComp.Instance.GetType()
                .GetField("_gridElement", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(dgComp.Instance)!;
            Context.JSInterop
              .Setup<Interop.BoundingClientRect>("mudElementRef.getBoundingClientRect", gridElement)
              .SetResult(new Interop.BoundingClientRect { Width = 50 });
            var colComps = comp.FindComponents<HeaderCell<DataGridHideAndResizeTest.Model>>();
            foreach (var colComp in colComps)
            {
                var col = colComp.Instance;
                if (!col.Column.HiddenState.Value)
                {
                    var headerElement = (ElementReference)col.GetType()
                        .GetField("_headerElement", BindingFlags.NonPublic | BindingFlags.Instance)!
                        .GetValue(col)!;
                    Context.JSInterop
                        .Setup<Interop.BoundingClientRect>("mudElementRef.getBoundingClientRect", headerElement)
                        .SetResult(new Interop.BoundingClientRect { Width = 50 });
                }
            }

            // Mouse click down
            var resizer = comp.FindAll(".mud-resizer").ElementAt(0);
            await comp.InvokeAsync(() => resizer.PointerDown());

            // Mouse move and release
            var resizeService = dgComp.Instance.ResizeService;
            var resizeServiceType = resizeService.GetType();
            var eventListener = (EventListener)resizeServiceType
                .GetField("_eventListener", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(resizeService);
            var upEventId = (Guid)resizeServiceType
                .GetField("_pointerUpSubscriptionId", BindingFlags.NonPublic | BindingFlags.Instance)!
                .GetValue(resizeService)!;
            await comp.InvokeAsync(async () => await eventListener!.OnEventOccur(upEventId, """{"ClientX":-10}"""));

            // Assert

            comp.FindAll("th").Count.Should().Be(2, "Two columns are displayed");
            comp.Find("th").GetStyle().Should().Contain(cssProp => cssProp.Name == "width", "The first column is resized");
        }

        [Test]
        public void QueryFilterExtensionTest()
        {
            var comp = Context.RenderComponent<DataGridFiltersTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFiltersTest.Model>>();

            var nameFilter = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Name"),
                Operator = FilterOperator.String.Contains,
                Value = "Sam",
            };
            var ageFilter = new FilterDefinition<DataGridFiltersTest.Model>
            {
                Column = dataGrid.Instance.GetColumnByPropertyName("Age"),
                Operator = FilterOperator.Number.GreaterThan,
                Value = 42,
            };

            var query = Array.Empty<DataGridFiltersTest.Model>().AsQueryable().Where([nameFilter, ageFilter]);
            query.ToString().Should().Match("*x.Name.Contains(*x.Age > Convert(42*");
        }

        [Test]
        public void QuerySortExtensionTest()
        {
            var nameSort = new SortDefinition<DataGridFiltersTest.Model>("Name", Descending: true, 0, default!);
            var ageSort = new SortDefinition<DataGridFiltersTest.Model>("Age", Descending: false, 1, default!);

            var query = Array.Empty<DataGridFiltersTest.Model>().AsQueryable().OrderBy([nameSort, ageSort]);
            query.ToString().Should().Be("MudBlazor.UnitTests.TestComponents.DataGrid.DataGridFiltersTest+Model[].OrderByDescending(x => x.Name).ThenBy(x => x.Age)");
        }

        [Test]
        public void QuerySortExtensionTestAscendingThenDescending()
        {
            var nameSort = new SortDefinition<DataGridFiltersTest.Model>("Name", Descending: false, 0, default!);
            var ageSort = new SortDefinition<DataGridFiltersTest.Model>("Age", Descending: true, 1, default!);

            var query = Array.Empty<DataGridFiltersTest.Model>().AsQueryable().OrderBy([nameSort, ageSort]);
            query.ToString().Should().Be("MudBlazor.UnitTests.TestComponents.DataGrid.DataGridFiltersTest+Model[].OrderBy(x => x.Name).ThenByDescending(x => x.Age)");
        }

        [Test]
        public void QuerySortExtensionTestEmptyDefinitions()
        {
            var source = Array.Empty<DataGridFiltersTest.Model>().AsQueryable();
            var query = source.OrderBy([]);
            query.Should().BeSameAs(source);
        }

        [Test]
        public void DataGridEnumLocalization()
        {
            var comp = Context.RenderComponent<DataGridFilterEnumLocalizationTest>();
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridFilterEnumLocalizationTest.Item>>();

            IElement FilterButton() => dataGrid.FindAll(".filter-button")[0];

            // click on the filter button
            FilterButton().Click();

            IElement SelectElement() => comp.Find("div.mud-select.filter-input");
            SelectElement().PointerDown();

            var items = comp.FindAll("div.mud-list-item").ToArray();

            items.Length.Should().Be(4);
            items[0].TextContent.Should().BeEmpty();
            items[1].TextContent.Should().Be("Free education");
            items[2].TextContent.Should().Be("Paid training");
            items[3].TextContent.Should().Be("Untranslated");
        }

        [Test]
        public void DataGridValidatorFormBinding()
        {
            var comp = Context.RenderComponent<DataGridValidatorTest>();
            var form = comp.FindComponent<MudForm>().Instance;
            var dataGrid = comp.FindComponent<MudDataGrid<DataGridValidatorTest.Item>>().Instance;
            dataGrid.Validator.Should().BeSameAs(form);

            var textField = comp.FindComponent<MudTextField<string>>();
            form.IsTouched.Should().BeFalse();
            form.IsValid.Should().BeFalse();

            // input valid value into text field
            textField.Find("input").Input("not empty");

            form.IsTouched.Should().BeTrue();
            form.IsValid.Should().BeTrue();

            // input invalid value into text field
            textField.Find("input").Input("");

            form.IsTouched.Should().BeTrue();
            form.IsValid.Should().BeFalse();
        }
    }
}
