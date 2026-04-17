using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Metroidvania.Session14
{
    public static class BuildPipelineCliAndCISample
    {
        [MenuItem("Session14/Build Steam Windows")]
        public static void BuildSteamWindows()
        {
            string[] scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = "Builds/Steam/Windows/Metroidvania.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.StrictMode
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            BuildReportSummaryToLog(report.summary.totalErrors);
            LogCiCdReference();
        }

        private static void BuildReportSummaryToLog(int errorCount)
        {
            if (errorCount > 0)
            {
                throw new Exception($"BuildPipeline failed with {errorCount} error(s).");
            }

            Debug.Log("BuildPipeline completed successfully for Steam Windows depot.");
            Debug.Log("CLI example: Unity -batchmode -quit -projectPath <path> -executeMethod Metroidvania.Session14.BuildPipelineCliAndCISample.BuildSteamWindows");
        }

        private static void LogCiCdReference()
        {
            string githubActionsYaml =
                "name: Steam Build\n" +
                "on:\n" +
                "  push:\n" +
                "    tags: ['v*.*.*']\n" +
                "jobs:\n" +
                "  build:\n" +
                "    runs-on: ubuntu-latest\n" +
                "    steps:\n" +
                "      - uses: actions/checkout@v4\n" +
                "      - name: Unity Build\n" +
                "        uses: game-ci/unity-builder@v4\n" +
                "      - name: Steam Upload\n" +
                "        run: steamcmd +login $STEAM_USERNAME $STEAM_PASSWORD +run_app_build scripts/app.vdf +quit\n";

            string cliReference =
                "GitHub Actions pipeline for Steam release\n" +
                "Unity CLI: -batchmode -executeMethod Metroidvania.Session14.BuildPipelineCliAndCISample.BuildSteamWindows\n" +
                "steamcmd command: steamcmd +run_app_build scripts/app.vdf +quit\n";

            Debug.Log(githubActionsYaml);
            Debug.Log(cliReference);
            Debug.Log("Reference logs include GitHub Actions, CLI, and steamcmd examples.");
        }
    }
}
