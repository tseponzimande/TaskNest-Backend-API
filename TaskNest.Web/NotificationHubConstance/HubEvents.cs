namespace TaskNest.API.NotificationHubConstance
{
    public static class HubEvents
    {
        #region Boards

        public const string BoardCreated = "Board_Created";
        public const string BoardUpdated = "Board_Updated";
        public const string BoardDeleted = "Board_Deleted";

        #endregion

        #region Column

        public const string ColumnCreated = "Board_ColumnCreated";
        public const string ColumnUpdated = "Board_ColumnUpdated";
        public const string ColumnDeleted = "Board_ColumnDeleted";
        public const string ColumnsReordered = "Board_ColumnsReordered";

        #endregion

        #region Task events

        public const string TaskCreated = "Board_TaskCreated";
        public const string TaskUpdated = "Board_TaskUpdated";
        public const string TaskDeleted = "Board_TaskDeleted";
        public const string TaskMoved = "Board_TaskMoved";

        #endregion

        #region Member events

        public const string MemberAdded = "Board_MemberAdded";
        public const string MemberRemoved = "Board_MemberRemoved";

        #endregion

        #region Helper to get board group name

        public static string GetBoardGroup(Guid boardId) => $"board_{boardId}";

        #endregion
    }
}