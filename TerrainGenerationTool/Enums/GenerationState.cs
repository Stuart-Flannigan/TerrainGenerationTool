public enum GenerationState
{
    NotStarted,
    GeneratingVerticies,
    GeneratingFaces,
    GeneratingNormals,
    GeneratingUVs,
    GeneratingFile,
    Cancelled,
    Completed
}