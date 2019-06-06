import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';

import axios from 'axios';
import Select from 'react-select';
import { stringify } from 'qs';
import { set } from 'lodash';
import Rating from 'react-rating';

import Breadcrumb from '../../shared/components/Breadcrumb';
import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import { PerformanceProcess } from '../../constants';

export default class Probation extends Component {
    constructor(props) {
        super(props);

        this.state = {
            probationer: null,
            employeeId: 406,
            evaluationDetails: [],
            evaluator: '',
            reviewer: ''
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl(`Probation/GetEvaluationDetail?employeeId=${this.state.employeeId}`)),
            axios(resolveClientUrl(`Probation/GetProbationers?employeeId=${this.state.employeeId}`))
        ];

        Promise.all(promises)
            .then(([evaluationDetailResponse, probationerResponse]) => {
                const probationer = probationerResponse.data[0];

                this.setState({
                    evaluationDetails: evaluationDetailResponse.data,
                    probationer: probationer,
                    evaluator: probationer.settings.find(x => x.processId === PerformanceProcess.Evaluator).evaluatorNickname,
                    reviewer: probationer.settings.find(x => x.processId === PerformanceProcess.Reviewer).evaluatorNickname
                });
            }).catch(axiosError);
    }

    updatePoint = (kpi, path, value) => {
        let evaluationDetails = this.state.evaluationDetails.slice();
        let kpiGroup = evaluationDetails.find(x => x.kpiGroupId === kpi.kpiGroupId);
        kpiGroup.details.find(x => x.kpiId === kpi.kpiId)[path] = value;

        this.setState({ evaluationDetails });
    }

    renderKPI = (kpi, index) => (
        <tr key={kpi.kpiId}>
            <td className="text-center">{index + 1}</td>
            <td>{kpi.kpiName}</td>
            <td>
                <Rating initialRating={kpi.selfPoint || 0}
                    emptySymbol="fa fa-star-o"
                    fullSymbol="fa fa-star"
                    onChange={value => this.updatePoint(kpi, 'selfPoint', value)}
                    readonly={!this.state.probationer.canEdit || this.state.probationer.currentProcessId !== PerformanceProcess.Self}
                />
            </td>
            <td>
                <Rating initialRating={kpi.evaluatorPoint || 0}
                    emptySymbol="fa fa-star-o"
                    fullSymbol="fa fa-star"
                    onChange={value => this.updatePoint(kpi, 'evaluatorPoint', value)}
                    readonly={!this.state.probationer.canEdit || this.state.probationer.currentProcessId !== PerformanceProcess.Evaluator}
                />
            </td>
        </tr>
    )

    renderKPIGroup = (kpiGroup) => (
        <Fragment key={kpiGroup.kpiGroupId}>
            <tr >
                <td colSpan={4}>{kpiGroup.name}</td>
            </tr>
            {kpiGroup.details.map(this.renderKPI)}
        </Fragment>
    )

    render() {
        if (this.state.probationer) {
            return (
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Probation Evaluation</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <table className="table table-bordered table-striped">
                            <thead>
                                <tr>
                                    <th colSpan={8}>Performance Result</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td className="text-center" colSpan={2}>Employee ({this.state.probationer.nickName})</td>
                                    <td className="text-center" colSpan={2}>Evaluator ({this.state.evaluator})</td>
                                    <td className="text-center" colSpan={2}>Reviewer ({this.state.reviewer})</td>
                                </tr>
                                <tr>
                                    <td>76/100</td>
                                    <td></td>
                                    <td>76/100</td>
                                    <td></td>
                                    <td>76/100</td>
                                    <td></td>
                                </tr>
                            </tbody>
                        </table>
                        <table className="table table-bordered table-striped">
                            <thead>
                                <tr>
                                    <th colSpan={4}>Detail Result</th>
                                </tr>
                                <tr>
                                    <th>#</th>
                                    <th>KPI</th>
                                    <th>Employee</th>
                                    <th>Evaluator</th>
                                </tr>
                            </thead>
                            <tbody>

                                {this.state.evaluationDetails.map(this.renderKPIGroup)}

                            </tbody>
                        </table>
                    </div>
                    <div className="col-sm-12">
                        <button className="btn btn-primary" onClick={this.handleSubmit}>Save</button>
                    </div>
                </div>
            );
        }

        return <Fragment />;
    }
}

ReactDOM.render(<Probation />, document.getElementById('probation-wrapper'));