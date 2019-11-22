import React, { PureComponent } from 'react';
import { groupBy } from '../../../common-functions';
import { PerformanceProcess, EmptyPlaceHolder } from '../../../constants';
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
            <span>TOEIC: {this.props.toeic ? `${this.props.toeic} ${toeicBonus}` : EmptyPlaceHolder}</span>
        );
    }

    getGrade = pr => {
        if (pr.processId === PerformanceProcess.ProjectManager) {
            return pr.grade;
        }

        if (pr.performanceRecord) {
            return pr.performanceRecord.grade ? pr.performanceRecord.grade.name : getResource('N/A');
        }

        return EmptyPlaceHolder;
    }

    compare = (process1, process2) => {
        if (process1.processId < process2.processId) {
            return -1;
        }

        if (process1.processId > process2.processId) {
            return 1;
        }

        return 0;
    }

    renderGradeSummary = () => {
        const processes = this.props.processes;

        const gradeSummary = processes.filter(x => x.processId !== PerformanceProcess.ProjectManager);
        if (processes.some(x => x.processId === PerformanceProcess.ProjectManager)) {
            const groupedPMProcess = groupBy(
                processes.filter(x => x.processId === PerformanceProcess.ProjectManager && x.performanceRecord && x.performanceRecord.grade),
                x => x.performanceRecord.grade.name);

            gradeSummary.push({
                processId: PerformanceProcess.ProjectManager,
                process: getResource('ProjectManager'),
                grade: groupedPMProcess.length > 0 ? groupedPMProcess.map(g => `${g[1].length}${g[0]}`).join(', ') : getResource('N/A')
            });
        } else {
            gradeSummary.push({
                processId: PerformanceProcess.ProjectManager,
                process: getResource('ProjectManager'),
                grade: EmptyPlaceHolder
            });
        }

        gradeSummary.sort(this.compare);

        return gradeSummary.map(x => (
            <span key={x.processId} className='grade-summary-item'>
                {x.process}: <strong>
                    {this.getGrade(x)}
                </strong>
            </span>
        ));
    }

    render() {
        return (
            <div className='grade-summary'>
                {this.renderGradeSummary()}
                {this.renderToeic()}
            </div>
        );
    }
}