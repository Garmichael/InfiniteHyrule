namespace ZeldaOverworldRandomizer.Common {
	public enum PathingNodeStatus {
		Unchecked,
		Closed,
		Open
	}

	public class PathingNode {
		public PathingNodeStatus Status = PathingNodeStatus.Unchecked;
		public int Id;
		public PathingNode ParentNode = null;
	}
}