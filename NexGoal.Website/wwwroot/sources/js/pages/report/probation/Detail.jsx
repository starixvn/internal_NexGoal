import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import moment from 'moment';
import axios from 'axios';
import { OverlayTrigger } from 'react-bootstrap';

import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import NexPanel from '../../../shared/components/NexPanel';
import Description from '../../probation/Description';
import HRM from '../../probation/HRM';
import EvaluationDetailTooltip from '../../probation/EvaluationDetailTooltip';

export default class Detail extends Component {
    constructor(props) {
        super(props);

        this.state = {
            employeeId: document.getElementById('employee-id').value,
            probationer: null,
            evaluationDetails: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('ProbationReport/GetByEmployeeId'), {
                params: {
                    id: this.state.employeeId
                }
            })
        ];

        Promise.all(promises)
            .then(([response]) => {
                const probationer = response.data.probationer;
                probationer.joinedDate = moment(probationer.joinedDate);
                probationer.avatar = `${document.getElementById('image-url').value}/${response.data.probationer.code}.jpg`;

                this.setState({
                    evaluationDetails: response.data.details,
                    probationer: probationer,
                    hrm: response.data.hrm
                });
            }).catch(axiosError);
    }

    renderKPI = (kpi, index) => {
        return (
            <tr key={kpi.kpiId}>
                <td>{kpi.kpiGroupId}-{index + 1}. {kpi.kpiName}</td>
                <td className="text-center"><strong>{kpi.selfPoint}</strong></td>
                <td className="text-center"><strong>{kpi.evaluatorPoint}</strong></td>
            </tr>
        );
    };

    renderKPIGroup = (kpiGroup, index) => {
        return (
            <Fragment key={kpiGroup.kpiGroupId}>
                <tr className="group-row">
                    <td>{index + 1}. {kpiGroup.name}</td>
                    <td className="text-center">{getResource('Employee')}</td>
                    <td className="text-center">{getResource('Evaluator')}</td>
                </tr>
                {kpiGroup.details.map(this.renderKPI)}
            </Fragment>
        );
    };

    renderPerformanceResult = isQualified => {
        return isQualified !== null &&
            <span className={isQualified ? 'passed' : 'failed'}>
                {isQualified ? 'Passed' : 'Failed'}
            </span>;
    };

    render() {
        if (this.state.probationer) {
            const probationerDetail = this.state.probationer;
            return (
                <div className="row">
                    <div className="main-content">
                        <NexPanel title="PERFORMANCE REVIEW: SKILLS AND ABILITIE">
                            <Description employeeInfo={probationerDetail} />
                        </NexPanel>

                        <NexPanel title="HRM System" enable={false}>
                            <HRM hrm={this.state.hrm} />
                        </NexPanel>
                        <NexPanel title="Performance Result">
                            <div className="result-table">
                                <table className="table table-bordered table-striped">
                                    <tbody>
                                        <tr className="group-row">
                                            <td><span>A. RESULT</span></td>
                                            <td colSpan="3" />
                                        </tr>
                                        <tr>
                                            <td className="text-center text-title">{getResource('Self')}</td>
                                            <td className="text-center text-title">{getResource('Score')}</td>
                                            <td className="text-center text-score">{probationerDetail.selfScore && `${probationerDetail.selfScore} pts`}</td>
                                            <td className="comment-cell comment-text">
                                                <p>{probationerDetail.selfComment}</p>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td className="text-center text-title" rowSpan={probationerDetail.canViewEvaluatorResult ? 2 : 1}>
                                                {getResource('Evaluator')}
                                                <div>({probationerDetail.evaluatorNickName})</div>
                                            </td>
                                            <td className="text-center text-title">{getResource('Score')}</td>
                                            <td className="text-center text-score">{probationerDetail.evaluatorScore && `${probationerDetail.evaluatorScore} pts`}</td>
                                            <td className="comment-cell comment-text" rowSpan={probationerDetail.canViewEvaluatorResult ? 2 : 1}>
                                                <p>{probationerDetail.evaluatorComment}</p>
                                            </td>
                                        </tr>
                                        {
                                            probationerDetail.canViewEvaluatorResult &&
                                            <tr>
                                                <td className="text-center text-title">{getResource('Result')}</td>
                                                <td className="text-center text-score">
                                                    {this.renderPerformanceResult(probationerDetail.isQualifiedByEvaluator)}
                                                </td>
                                            </tr>
                                        }
                                        <tr>
                                            <td className="text-center text-title">
                                                {getResource('Reviewer')}
                                                <div>({probationerDetail.reviewerNickName})</div>
                                            </td>
                                            <td className="text-center text-title">{getResource('FinalResult')}</td>
                                            <td className="text-center text-score">
                                                {this.renderPerformanceResult(probationerDetail.isQualified)}
                                            </td>
                                            <td className="comment-cell comment-text">
                                                <p>{probationerDetail.reviewerComment}</p>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <table className="table table-bordered table-striped">
                                <tbody>
                                    <tr className="group-row">
                                        <td colSpan="3">
                                            B. DETAILS
                                            <OverlayTrigger placement="bottom" overlay={EvaluationDetailTooltip}>
                                                <i className="icon-info" />
                                            </OverlayTrigger>
                                        </td>
                                    </tr>
                                    {this.state.evaluationDetails.map(this.renderKPIGroup)}
                                    <tr className="score">
                                        <td className="text-center"><strong>{getResource('Score')}</strong></td>
                                        <td className="text-center">
                                            {
                                                probationerDetail.selfScore &&
                                                <span className="score-badge"><span>{probationerDetail.selfScore}</span>/100 pts</span>
                                            }
                                        </td>
                                        <td className="text-center">
                                            {
                                                probationerDetail.evaluatorScore &&
                                                <span className="score-badge"><span>{probationerDetail.evaluatorScore}</span>/100 pts</span>
                                            }
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </NexPanel>

                        <a className="btn btn-primary" href={resolveClientUrl('ProbationReport/Index')}>Back</a>
                    </div >
                </div >
            );
        }

        return <Fragment />;
    }
}

ReactDOM.render(<Detail />, document.getElementById('probation-report-detail-wrapper'));