using Grasshopper.Kernel;
using ParallelCore.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPGHAdapter.Wrapper
{
    public class GraphCreator
    {
        private GH_Document _ghDoc;
        public GraphCreator(GH_Document doc)
        {
            _ghDoc = doc;
            CreateFromGhDoc();
        }

        public ICollection<ITopologicalNode> Nodes { get; private set; }

        private List<EditableTopologicalNode> _nodes = new List<EditableTopologicalNode>();
        private Dictionary<Guid, EditableTopologicalNode> _hashtable = new Dictionary<Guid, EditableTopologicalNode>();
        private void AddObjects()
        {
            EditableTopologicalNode node;

            foreach (var it in _ghDoc.Objects)
            {
                switch (it)
                {
                    case GH_Component cmp:
                        node = new Component(cmp);
                        _nodes.Add(node);
                        _hashtable[cmp.InstanceGuid] = node;
                        break;

                    case IGH_Param param:
                        node = new Param(param);
                        _nodes.Add(node);
                        _hashtable[param.InstanceGuid] = node;
                        break;

                    default:
                        continue;
                }
            }
        }
        private void GenerateTopologicalRelations()
        {
            foreach (var it in _nodes)
            {
                switch (it)
                {
                    case Component cmp:
                        cmp.SourceList.AddRange(
                            cmp.MappedComponent.Params.Input
                                .SelectMany(p => p.Sources)
                                .Select(p => p.Attributes.GetTopLevel.DocObject.InstanceGuid)
                                .Distinct()
                                .Select(id => _hashtable[id])
                            );
                        break;

                    case Param param:
                        param.SourceList.AddRange(
                            param.MappedParam.Sources
                                .Select(p => p.Attributes.GetTopLevel.DocObject.InstanceGuid)
                                .Distinct()
                                .Select(id => _hashtable[id])
                            );
                        break;
                }
            }
        }
        private void CreateFromGhDoc()
        {
            AddObjects();
            GenerateTopologicalRelations();

            Nodes = _nodes.Cast<ITopologicalNode>().ToArray();
        }
    }
}
