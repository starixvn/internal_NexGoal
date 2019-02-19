import React, { Component, Fragment } from 'react';
import { PerformanceProcess, PAStatus, notAvailableGradeTooltip } from '../../../../constants';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';
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
        if (toeicPolicy !== null && toeicPolicy > 0) toeicClass = 'toeic-passed';
        if (toeicPolicy === null || toeicPolicy < 0) toeicClass = 'toeic-failed';
        return <span className={`bold ${toeicClass}`}>{toeic ? toeic : getResource('N/A')}</span>;
    }

    parseIntGradeId = (gradeId) => {
        return gradeId !== null ? gradeId : 0;
    }

    renderNotAvailableGrade = () => {
        const tooltip = (
            <Tooltip className="focus-note">
                <p>{notAvailableGradeTooltip}</p>
            </Tooltip>
        );
        return (
            <OverlayTrigger placement="top" overlay={tooltip} >
                <span className="pointer red">{getResource('N/A')}</span>
            </OverlayTrigger>
        );
    }

    quickEditPA = (paRecordId, isRevised) => {
        this.props.onQuickEditPA(paRecordId, isRevised);
    }

    countDuplicateGrades = (grades) => {
        grades.sort();
        let displayGrades = [];
        let currentGrade = null;
        var count = 0;
        for (let i = 0; i < grades.length; i++) {
            if (grades[i] !== currentGrade) {
                if (count > 0) {
                    displayGrades.push(count > 1 ? count + currentGrade : currentGrade);
                }
                currentGrade = grades[i];
                count = 1;
            } else {
                count++;
            }
        }
        if (count > 0) {
            displayGrades.push(count > 1 ? count + currentGrade : currentGrade);
        }

        return displayGrades.join(', ');
    }

    renderSelfGrade = (paProcesses) => {
        const processes = paProcesses.filter(process => process.processId === PerformanceProcess.Self);
        let grade = '';
        if (processes[0].performanceRecord) {
            grade = processes[0].performanceRecord.grade ? processes[0].performanceRecord.grade.name : getResource('N/A');
        }

        return <span>{grade}</span>;
    }

    renderPMGrades = (paProcesses) => {
        const processes = paProcesses.filter(process => process.processId === PerformanceProcess.ProjectManager);
        const grades = processes.map(process => {
            return process.performanceRecord && process.performanceRecord.grade ?
                process.performanceRecord.grade.name : getResource('N/A');
        });
        const tooltip = (
            <Tooltip className="focus-note">
                {
                    processes.map(process => {
                        return (
                            <p>
                                {
                                    `${process.evaluator.nickName}: ${process.performanceRecord && process.performanceRecord.grade
                                        ? process.performanceRecord.grade.name : getResource("N/A")} `
                                }
                            </p>
                        );
                    })
                }
            </Tooltip>
        );

        return (
            <OverlayTrigger placement="top" overlay={tooltip}>
                <span className="pointer">{this.countDuplicateGrades(grades)}</span>
            </OverlayTrigger>
        );
    }

    renderEvaluatorGrade = (paProcesses, statusId) => {
        const evaluatorProcess = paProcesses.filter(process => process.processId === PerformanceProcess.Evaluator)[0];
        const naClass = statusId === PAStatus.InProgress ? 'red' : '';
        if (evaluatorProcess.performanceRecord && evaluatorProcess.performanceRecord.grade) {
            return <span>{evaluatorProcess.performanceRecord.grade.name}</span>;
        }

        return <span className={naClass}>{getResource('N/A')}</span>;
    }

    renderPAForReviewer = (record) => {
        let reviewerProcess = record.processes.filter(process => process.processId === PerformanceProcess.Reviewer)[0];
        let gradeChangeClass = '';
        if (this.parseIntGradeId(record.gradeId) > this.parseIntGradeId(record.originalGradeId)) {
            gradeChangeClass = 'icon-increase-arrow';
        }

        if (this.parseIntGradeId(record.gradeId) < this.parseIntGradeId(record.originalGradeId)) {
            gradeChangeClass = 'icon-descrease-arrow';
        }

        let paGrade = <span>{record.grade ? record.grade.name : getResource('N/A')}</span>;
        if (record.statusId === PAStatus.InProgress) {
            const displayGrade = record.grade ? <span>{record.grade.name}</span> : this.renderNotAvailableGrade();
            paGrade = <a onClick={() => this.handleClick(record.paRecordId)}>{displayGrade}</a>;
        }

        const noteClass = reviewerProcess.performanceRecord ? 'reviewer-seen' : 'reviewer-notyetseen';
        return (
            <Fragment>
                <span className={`pa-grade text-center ${noteClass}`}>{paGrade}
                    <span className={gradeChangeClass} />
                </span>
            </Fragment>
        );
    };

    renderStatus = (isEdited, paRecordId, paStatusId) => {
        const isClosed = paStatusId === PAStatus.Closed;
        let status;
        if (isClosed) {
            const checked = isEdited ? 'icon-check' : '';
            status = <span className={checked} />;
        } else {
            status = (
                <div>
                    <input type="checkbox" className="checkbox-custom"
                        id={`chb-${paRecordId}`}
                        checked={isEdited}
                        onClick={() => this.quickEditPA(paRecordId, isEdited)}
                    />
                    <label htmlFor={`chb-${paRecordId}`} />
                </div>
            );
        }

        return status;
    }

    renderNickName = (record) => {
        let displayNickname = <span>{record.employee.nickName}</span>;
        if (record.statusId !== PAStatus.InProgress) {
            displayNickname = <a onClick={() => this.handleClick(record.paRecordId)}>{record.employee.nickName}</a>;
        }
        return displayNickname;
    }

    handleClick = (paRecordId) => {
        this.props.onClickEditPA(paRecordId);
    }

    render() {
        const performanceRecords = this.props.data.performanceAppraisals;
        return (
            <Fragment>
                <tr className="group-row">
                    <td colSpan={this.props.colSpan}>{this.props.data.groupName}</td>
                </tr>
                {
                    performanceRecords &&
                    performanceRecords.map(record => {
                        return (
                            <tr>
                                <td>{this.renderNickName(record)}</td>
                                <td className="text-center">{this.renderToeic(record.toeicPolicy, record.toeic)}</td>
                                <td className="text-center">{this.renderSelfGrade(record.processes)}</td>
                                <td className="text-center">
                                    {this.renderPMGrades(record.processes)}
                                </td>
                                <td className="text-center">
                                    {this.renderEvaluatorGrade(record.processes, record.statusId)}
                                </td>
                                <td className="">{this.renderPAForReviewer(record)}</td>
                                <td className="text-center">
                                    {this.renderStatus(record.isEdited, record.paRecordId, record.statusId)}
                                </td>
                            </tr>
                        );
                    })
                }
            </Fragment>
        );
    }
}

export default Rows;