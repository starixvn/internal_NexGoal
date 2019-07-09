import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';

import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import NexPanel from '../../../shared/components/NexPanel';

export default class Detail extends Component {
    constructor(props) {
        super(props);

        this.state = {
            employeeId: document.getElementById('employee-id').value,
            probationer: null,
            evaluationDetails: []
        };

        this.imageUrl = document.getElementById('image-url').value;
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
                this.setState({
                    evaluationDetails: response.data.details,
                    probationer: response.data.probationer,
                });
            }).catch(axiosError);
    }

    renderKPI = (kpi, index) => (
        <tr key={kpi.kpiId}>
            <td>{kpi.kpiGroupId}-{index + 1}. {kpi.kpiName}</td>
            <td className="text-center"><strong>{kpi.selfPoint}</strong></td>
            <td className="text-center"><strong>{kpi.evaluatorPoint}</strong></td>
        </tr>
    )

    renderKPIGroup = (kpiGroup, index) => (
        <Fragment key={kpiGroup.kpiGroupId}>
            <tr className="group-row">
                <td>{index + 1}. {kpiGroup.name}</td>
                <td className="text-center">{getResource('Employee')}</td>
                <td className="text-center">{getResource('Evaluator')}</td>
            </tr>
            {kpiGroup.details.map(this.renderKPI)}
        </Fragment>
    )

    render() {
        if (this.state.probationer) {
            return (
                <div className="row">
                    <div className="main-content">
                        <NexPanel title="Performance Result">
                            <table className="table table-bordered table-striped result-table">
                                <tbody>
                                    <tr className="group-row">
                                        <td colSpan="4">A. RESULT</td>
                                    </tr>
                                    <tr>
                                        <td className="text-center text-title">{getResource('Self')}</td>
                                        <td className="text-center text-title">{getResource('Score')}</td>
                                        <td className="text-center text-score">{this.state.probationer.selfScore && `${this.state.probationer.selfScore} pts`}</td>
                                        <td className="comment-cell comment-text">
                                            <p>{this.state.probationer.selfComment}</p>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td className="text-center text-title" rowSpan={this.state.probationer.canViewEvaluatorResult ? 2 : 1}>{getResource('Evaluator')}</td>
                                        <td className="text-center text-title">{getResource('Score')}</td>
                                        <td className="text-center text-score">{this.state.probationer.evaluatorScore && `${this.state.probationer.evaluatorScore} pts`}</td>
                                        <td className="comment-cell comment-text" rowSpan={this.state.probationer.canViewEvaluatorResult ? 2 : 1}>
                                            <p>{this.state.probationer.evaluatorComment}</p>
                                        </td>
                                    </tr>
                                    {this.state.probationer.canViewEvaluatorResult &&
                                        <tr>
                                            <td className="text-center text-title">{getResource('Result')}</td>
                                            <td className="text-center text-score">
                                                {this.state.probationer.isQualifiedByEvaluator !== null ?
                                                    <span className={this.state.probationer.isQualifiedByEvaluator ? 'passed' : 'failed'}>
                                                        {this.state.probationer.isQualifiedByEvaluator ? 'Passed' : 'Failed'}
                                                    </span>
                                                    : '-'
                                                }
                                            </td>
                                        </tr>
                                    }
                                    <tr>
                                        <td className="text-center text-title">{getResource('Reviewer')}</td>
                                        <td className="text-center text-title">{getResource('FinalResult')}</td>
                                        <td className="text-center text-score">
                                            {this.state.probationer.isQualified !== null ?
                                                <span className={this.state.probationer.isQualified ? 'passed' : 'failed'}>
                                                    {this.state.probationer.isQualified ? 'Passed' : 'Failed'}
                                                </span>
                                                : '-'
                                            }
                                        </td>
                                        <td className="comment-cell comment-text">
                                            <p>{this.state.probationer.reviewerComment}</p>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <table className="table table-bordered table-striped">
                                <tbody>

                                    <tr className="group-row">
                                        <td colSpan="3">
                                            B. DETAILS
                                    </td>
                                    </tr>
                                    {this.state.evaluationDetails.map(this.renderKPIGroup)}
                                    <tr className="score">
                                        <td className="text-center"><strong>Score</strong></td>
                                        <td className="text-center">
                                            {this.state.probationer.selfScore &&
                                                <span className="score-badge"><span>{this.state.probationer.selfScore}</span>/100 pts</span>
                                            }
                                        </td>
                                        <td className="text-center">
                                            {this.state.probationer.evaluatorScore &&
                                                <span className="score-badge"><span>{this.state.probationer.evaluatorScore}</span>/100 pts</span>
                                            }
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </NexPanel>
                    </div>
                </div>
            );
        }

        return <Fragment />
    }
}

ReactDOM.render(<Detail />, document.getElementById('probation-report-detail-wrapper'));