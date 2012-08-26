using NQuery.Language.Binding;

namespace NQuery.Language
{
    public sealed class Compilation
    {
        private readonly SyntaxTree _syntaxTree;
        private readonly DataContext _dataContext;

        public Compilation(SyntaxTree syntaxTree, DataContext dataContext)
        {
            _syntaxTree = syntaxTree;
            _dataContext = dataContext;
        }

        public SemanticModel GetSemanticModel()
        {
            var binder = new Binder(_dataContext);
            var bindingResult = binder.Bind(_syntaxTree.Root);
            return new SemanticModel(this, bindingResult);
        }

        public Compilation WithSyntaxTree(SyntaxTree syntaxTree)
        {
            return _syntaxTree == syntaxTree ? this : new Compilation(syntaxTree, _dataContext);
        }

        public Compilation WithDataContext(DataContext dataContext)
        {
            return _dataContext == dataContext ? this : new Compilation(_syntaxTree, dataContext);
        }

        public static Compilation Empty = new Compilation(SyntaxTree.Empty, DataContext.Empty);

        public SyntaxTree SyntaxTree
        {
            get { return _syntaxTree; }
        }

        public DataContext DataContext
        {
            get { return _dataContext; }
        }
    }
}