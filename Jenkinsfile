@Library("JenkinsPipelineLibrary") _

pipeline {
  agent any

  options{
    buildDiscarder(logRotator(numToKeepStr: '5'))
    disableConcurrentBuilds()
    gitLabConnection('Gitlab')
  }

  triggers{
    gitlab(triggerOnPush: true, triggerOnMergeRequest: true, branchFilterType: 'All')
  }

  stages {
    stage('Build') {
      steps {
        script{
          pipelineLib.beginSonarQubeForMsBuild("NexGoal", "Nexus / NexGoal", "/d:sonar.cs.opencover.reportsPaths=\"${WORKSPACE}\\CoverageReports\\*.xml\" /d:sonar.cs.vstest.reportsPaths=\"${WORKSPACE}\\TestResults\\*.trx\"")

          pipelineLib.restoreNuget()

          pipelineLib.msBuild("NexGoal.sln")
        }
      }
    }
	
	stage("Unit Test"){
      steps{
        script{
          pipelineLib.xUnitForNetCore()
        }
      }
    }

    stage("SonarQube Analysis"){
      steps{
        script{
          pipelineLib.endSonarQubeForMsBuild()
        }
      }

      post{
        always{
          script{
            if(manager.logContains(".*Quality gate status.*")){
              pipelineLib.generateSonarQubeReport("NexGoal")
            }
          }
		  
		  archiveArtifacts "*.xml,*.email"
		  step([$class: 'ACIPluginPublisher', name: '*.xml', shownOnProjectPage: false])
		  mstest failOnError: false
        }
      }
    }
  }

  post{
    unsuccessful{
      emailext body: '$DEFAULT_CONTENT', subject: '$DEFAULT_SUBJECT', to: 'quinn.vo@starixsoft.com,tim.trinh@starixsoft.com,elton.ton@starixsoft.com,oliver.dang@starixsoft.com'
    }
  }
}