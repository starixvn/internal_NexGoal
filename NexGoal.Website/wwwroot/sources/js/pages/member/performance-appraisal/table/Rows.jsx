import React, { Component, Fragment } from 'react';
import isEqual from 'react-fast-compare';

import { PerformanceProcess, PAStatus, notAvailableGradeTooltip, EmptyPlaceHolder } from '../../../../constants';
import { getPAGradeClassName } from '../../../../common-functions';
import { Tooltip, OverlayTrigger } from 'react-bootstrap';
import getResource from '../../../../resourceHelper';

class Rows extends Component {
    shouldComponentUpdate(nextProps) {
        return !isEqual(this.props, nextProps);
    }

    renderToeic = (toeicPolicy, toeic) => {
        let toeicClass = '';
        if (toeicPolicy === null || toeicPolicy < 0) {
            toeicClass = 'toeic-failed';
        }

        return <span className={`bold ${toeicClass}`}>{toeic ? toeic : EmptyPlaceHolder}</span>;
    };

    renderNotAvailableGrade = paRecordId => {
        const tooltip = <Tooltip className="focus-note" id={`notAvailable-${paRecordId}`}>
            <p>{notAvailableGradeTooltip}</p>
        </Tooltip>;

        return <OverlayTrigger placement="top" overlay={tooltip} >
            <span className="pointer red">{getResource('N/A')}</span>
        </OverlayTrigger>;
    };

    quickEditPA = (paRecordId, isRevised) => {
        this.props.onQuickEditPA(paRecordId, isRevised);
    };

    countDuplicateGrades = grades => {
        grades.sort();
        const displayGrades = [];
        let currentGrade = null;
        let count = 0;
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

        return displayGrades.length > 0 ? displayGrades.join(', ') : '';
    };

    renderSelfGrade = paProcesses => {
        const selfProcess = paProcesses.filter(process => process.processId === PerformanceProcess.Self)[0];
        return <span>{selfProcess.grade ? selfProcess.grade : EmptyPlaceHolder}</span>;
    };

    renderPMGradesPerformanceAppraisal = (paProcessesPA, paRecordIdPA) => {
        const processes = paProcessesPA.filter(process => process.grade && process.processId === PerformanceProcess.ProjectManager);
        if (processes.length === 0) {
            return EmptyPlaceHolder;
        }

        const gradesPA = processes.map(process => process.grade);
        const tooltipPA = <Tooltip className="focus-note" id={`pmProcess-${paRecordIdPA}`}>
            {
                processes.map((process, index) => {
                    if (process.grade) {
                        return <p key={index}>{`${process.evaluator.nickName}: ${process.grade}`}</p>;
                    }

                    return null;
                })
            }
        </Tooltip>;

        return <OverlayTrigger placement="top" overlay={tooltipPA}>
            <span className="pointer">{this.countDuplicateGrades(gradesPA)}</span>
        </OverlayTrigger>;
    };

    renderEvaluatorGrade = (paProcesses, statusId) => {
        const evaluatorProcess = paProcesses.filter(process => process.processId === PerformanceProcess.Evaluator)[0];
        let grade = EmptyPlaceHolder;

        if (evaluatorProcess.grade) {
            const naClass = evaluatorProcess.grade === 'N/A' && statusId === PAStatus.InProgress ? 'red' : '';
            grade = <span className={naClass}>{evaluatorProcess.grade}</span>;
        }

        return grade;
    };

    renderPAGrade = record => {
        const gradeId = record.grade ? record.grade.gradeId : null;
        const originalGradeId = record.originalGrade ? record.originalGrade.gradeId : null;
        const gradeChangeClass = getPAGradeClassName(originalGradeId, gradeId);

        let paGrade = <span>{record.grade ? record.grade.name : getResource('N/A')}</span>;
        if (record.statusId === PAStatus.InProgress) {
            const displayGrade = record.grade ? <span>{record.grade.name}</span> : this.renderNotAvailableGrade(record.paRecordId);
            paGrade = <a onClick={() => this.handleClick(record.paRecordId)}>{displayGrade}</a>;
        }

        return <span className="pa-grade text-center">{paGrade}
            <span className={gradeChangeClass} />
        </span>;
    };

    renderStatus = (isRevised, paRecordId, paStatusId) => {
        const isClosed = paStatusId === PAStatus.Closed;
        let status;
        if (isClosed) {
            const checked = isRevised ? 'icon-check' : '';
            status = <span className={checked} />;
        } else {
            status = <div>
                <input type="checkbox" className="checkbox-custom"
                    id={`chb-${paRecordId}`}
                    checked={isRevised}
                    onChange={() => this.quickEditPA(paRecordId, isRevised)}
                />
                <label htmlFor={`chb-${paRecordId}`} />
            </div>;
        }

        return status;
    };

    renderReviewerGrade = paProcesses => {
        const processes = paProcesses.filter(process => process.processId === PerformanceProcess.Reviewer);
        if (!processes.length) {
            return EmptyPlaceHolder;
        }

        return processes[0].grade ? processes[0].grade : EmptyPlaceHolder;
    };

    renderNickName = record => {
        let displayNickname = <span>{record.employee.nickName}</span>;
        if (record.statusId !== PAStatus.InProgress) {
            displayNickname = <a onClick={() => this.handleClick(record.paRecordId)}>{record.employee.nickName}</a>;
        }

        return displayNickname;
    };

    handleClick = paRecordId => {
        this.props.onClickEditPA(paRecordId);
    };

    render() {
        const performanceRecords = this.props.data.performanceAppraisals;
        return (
            <Fragment>
                <tr className="group-row">
                    <td colSpan={this.props.colSpan}>{this.props.data.groupName}</td>
                </tr>
                {
                    performanceRecords &&
                    performanceRecords.map((record, index) => {
                        return (
                            <tr key={index}>
                                <td>{this.renderNickName(record)}</td>
                                <td className="text-center">{this.renderToeic(record.toeicPolicy, record.toeic)}</td>
                                <td className="text-center">{this.renderSelfGrade(record.processes)}</td>
                                <td className="text-center">
                                    {this.renderPMGradesPerformanceAppraisal(record.processes, record.paRecordId)}
                                </td>
                                <td className="text-center">
                                    {this.renderEvaluatorGrade(record.processes, record.statusId)}
                                </td>
                                <td className="text-center">{this.renderReviewerGrade(record.processes)}</td>
                                <td className="text-center">{this.renderPAGrade(record)}</td>
                                <td className="text-center">
                                    {this.renderStatus(record.isRevised, record.paRecordId, record.statusId)}
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