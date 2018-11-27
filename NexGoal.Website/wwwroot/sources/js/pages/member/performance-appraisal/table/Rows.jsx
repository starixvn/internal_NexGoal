import React, { Component, Fragment } from 'react';
import { PerformanceProcess } from '../../../../constants';
import getResource from '../../../../resourceHelper';

class Rows extends Component {
    constructor(props) {
        super(props);
    }

    shouldComponentUpdate(nextProps) {
        if (JSON.stringify(this.props.data) !== JSON.stringify(nextProps.data)) {
            return true;
        }

        return false;
    }

    renderToeic = (toeicPolicy, toeic) => {
        let toeicClass = '';
        if (toeicPolicy !== null && toeicPolicy > 0) toeicClass = 'role-member';
        if (toeicPolicy === null || toeicPolicy < 0) toeicClass = 'red';
        return <span className={`bold ${toeicClass}`}>{toeic ? toeic : getResource('N/A')}</span>;
    }

    renderPAForReviewer = (record) => {
        let reviewerProcess = record.processes.filter(process => process.processId === PerformanceProcess.Reviewer);
        let paGrade;
        let gradeChangeClass = '';
        if (record.isEdited) {
            paGrade = record.grade.name;
        } else {
            paGrade = record.originalGrade.name;
        }
        if (record.gradeId < record.originalGradeId) gradeChangeClass = 'descrease';
        if (record.gradeId > record.originalGradeId) gradeChangeClass = 'increase';

        return (
            <Fragment>
                {
                    reviewerProcess.length > 0 &&
                    <span className="fa fa-pencil-square-o" />
                }
                <a onClick={() => this.handleClick(record.paRecordId)}>{paGrade}</a>
                <span className={gradeChangeClass} />
            </Fragment>
        );
    };

    renderPAByProcess = (paProcesses, processId) => {
        let processes = paProcesses.filter(process => process.processId === processId);
        let grade = processes.map(process => {
            return process.performanceRecord ? process.performanceRecord.grade.name : '';
        }).join(', ');

        return <span>{grade}</span>;
    };

    renderStatus = (isEdited, paRecordId) => {
        if (isEdited) {
            return <span className="checked-stick" />;
        } else {
            return <span onClick={() => this.props.onQuickEditPA(paRecordId)} >Click</span>;
        }
    }

    handleClick = (paRecordId) => {
        this.props.onClickEditPA(paRecordId);
    }

    render() {
        const performanceRecords = this.props.data.performanceAppraisals;
        return (
            <Fragment>
                <tr>
                    <td colSpan={this.props.colSpan}>{this.props.data.groupName}</td>
                </tr>
                {
                    performanceRecords &&
                    performanceRecords.map(record => {
                        return (
                            <tr>
                                <td>{record.employee.nickName}</td>
                                <td>{this.renderToeic(record.toeicPolicy, record.toeic)}</td>
                                <td>{this.renderPAByProcess(record.processes, PerformanceProcess.Self)}</td>
                                <td>{this.renderPAByProcess(record.processes, PerformanceProcess.ProjectManager)}</td>
                                <td>{this.renderPAByProcess(record.processes, PerformanceProcess.Evaluator)}</td>
                                <td>{this.renderPAForReviewer(record)}</td>
                                <td>{this.renderStatus(record.isEdited, record.paRecordId)}</td>
                            </tr>
                        );
                    })
                }
            </Fragment>
        );
    }
}

export default Rows;