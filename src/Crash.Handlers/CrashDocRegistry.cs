using Crash.Common.Document;

using Rhino;

namespace Crash.Handlers
{

	// TODO : Is this needed?
	public sealed partial class CrashDocRegistry
	{
		static BidirectionalMap.BiMap<RhinoDoc, CrashDocumentState> DocumentRelationship;


		/// <summary>The Active Crash Document.</summary>
		public static CrashDoc? ActiveDoc => GetRelatedDocument(RhinoDoc.ActiveDoc);
		// TODO : Make internal
		public static CrashDocumentState ActiveState => DocumentRelationship.Forward[RhinoDoc.ActiveDoc];

		static CrashDocRegistry()
		{
			DocumentRelationship = new BidirectionalMap.BiMap<RhinoDoc, CrashDocumentState>();
			RhinoDoc.ActiveDocumentChanged += RhinoDoc_ActiveDocumentChanged;
		}

		private static void RhinoDoc_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			// ... 
		}

		public static CrashDoc? GetRelatedDocument(RhinoDoc doc)
		{
			if (DocumentRelationship.Forward.ContainsKey(doc))
				return DocumentRelationship.Forward[doc].Document;

			return null;
		}

		public static RhinoDoc? GetRelatedDocument(CrashDoc doc)
		{
			foreach (var kvp in DocumentRelationship.Reverse)
			{
				if (kvp.Key.Document.Equals(doc))
				{
					return kvp.Value;
				}
			}

			return null;
		}

		public static IEnumerable<CrashDoc> GetOpenDocuments()
			=> DocumentRelationship.Forward.Values.Select(s => s.Document);

		public static CrashDoc CreateAndRegisterDocument(RhinoDoc rhinoDoc)
		{
			if (DocumentRelationship.Forward.ContainsKey(rhinoDoc))
			{
				return DocumentRelationship.Forward[rhinoDoc].Document;
			}

			CrashDocumentState state = Create();
			Register(state, rhinoDoc);

			return state.Document;
		}

		private static CrashDocumentState Create()
		{
			var crashDoc = new CrashDoc();
			var state = new CrashDocumentState(crashDoc);

			return state;
		}

		private static void Register(CrashDocumentState crashDocState,
									RhinoDoc rhinoDoc)
		{
			DocumentRelationship.Add(rhinoDoc, crashDocState);
		}

	}

}
