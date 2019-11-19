import React, { PureComponent } from 'react';
import { groupBy } from '../../../common-functions';
import { PerformanceProcess } from '../../../constants';
import getResource from '../../../resourceHelper';

export default class GradeSummary extends PureComponent {
    constructor(props) {
        super(props);
    }

    renderToeic = () => {
        const toeicPolicy = this.props.toeicPolicy;
        let toeicBonus = '';

        if (toeicPolicy > 0) {
            toeicBonus = '(+1 grade)';
        }

        if (toeicPolicy < 0) {
            toeicBonus = '(-1 grade)';
        }

        return (
            <span>TOEIC: {this.props.toeic} {toeicBonus}</span>
        );
    }

    getGrade = pr => {
        if (pr.processId === PerformanceProcess.ProjectManager) {
            return pr.grade;
        }

        if (pr.performanceRecord && pr.performanceRecord.grade) {
            return pr.performanceRecord.grade.name;
        }

        return getResource('N/A');
    }

    render() {
        const processes = this.props.processes;
        const groupedPMProcess = groupBy(
            processes.filter(x => x.processId === PerformanceProcess.ProjectManager && x.performanceRecord && x.performanceRecord.grade),
            x => x.performanceRecord.grade.name);

        const renderGradeSummary = processes.filter(x => x.processId !== PerformanceProcess.ProjectManager);
        renderGradeSummary.splice(1, 0, {
            processId: PerformanceProcess.ProjectManager,
            process: getResource('ProjectManager'),
            grade: groupedPMProcess.length > 0 ? groupedPMProcess.map(g => `${g[1].length}${g[0]}`).join(', ') : getResource('N/A')
        });

        return (
            <div className='grade-summary'>
                {
                    renderGradeSummary.map(x => (
                        <span key={x.processId} className='grade-summary-item'>
                            {x.process}: <strong>
                                {this.getGrade(x)}
                            </strong>
                        </span>
                    ))
                }
                {this.renderToeic()}
            </div>
        );
    }
}