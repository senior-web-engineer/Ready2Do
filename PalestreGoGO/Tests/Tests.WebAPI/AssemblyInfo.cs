using Tests.Utils;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCollectionOrderer(CustomTestCollectionOrderer.TypeName, CustomTestCollectionOrderer.AssembyName)]
[assembly: TestCaseOrderer(CustomTestCaseOrderer.TypeName, CustomTestCaseOrderer.AssembyName)]