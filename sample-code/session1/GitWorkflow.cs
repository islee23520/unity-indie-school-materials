/*
 * Session 1: Git Workflow Context
 * 
 * Keywords: git init, git add, git commit, git push
 * 
 * Typical Workflow:
 * 1. git init          - Initialize a new repository
 * 2. git add .         - Stage all changes
 * 3. git commit -m ""  - Create a snapshot of staged changes
 * 4. git push          - Upload local commits to remote
 */

namespace Metroidvania.Session1
{
    public class GitWorkflow
    {
        // This file serves as a reference for the Git workflow discussed in Session 1.
        // In a real project, you would use the terminal or a Git client.
        
        public string GetCommitMessage()
        {
            return "feat: implement basic player movement with Rigidbody2D";
        }
    }
}
