import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';

import axios from 'axios';
import Select from 'react-select';
import { stringify } from 'qs';
import { set } from 'lodash';
import Rating from 'react-rating';
import { toast } from 'react-toastify';

import Breadcrumb from '../../shared/components/Breadcrumb';
import NexPanel from '../../shared/components/NexPanel';
import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import { PerformanceProcess } from '../../constants';
import ToastLayout from '../../shared/layouts/ToastLayout';

export default class Probation extends Component {
    constructor(props) {
        super(props);

        this.state = {
            probationer: null,
            employeeId: 406,
            evaluationDetails: [],
            evaluator: '',
            reviewer: '',
            avatarUrl: document.getElementById('employeeInfo-avatar').getAttribute('src')
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('MyProbation/GetEvaluationDetail')),
            axios(resolveClientUrl('Probation/GetEvaluation'))
        ];

        Promise.all(promises)
            .then(([evaluationDetailResponse, probationerResponse]) => {
                const probationer = probationerResponse.data;

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
            <td>{index + 1}. {kpi.kpiName}</td>
            <td className="text-center">
                <Rating initialRating={kpi.selfPoint || 0}
                    emptySymbol="fa fa-star-o"
                    fullSymbol="fa fa-star"
                    onChange={value => this.updatePoint(kpi, 'selfPoint', value)}
                    readonly={!this.state.probationer.canEdit}
                />
            </td>
        </tr>
    )

    renderKPIGroup = (kpiGroup, index) => (
        <Fragment key={kpiGroup.kpiGroupId}>
            <tr className="group-row">
                <td>{index + 1}. {kpiGroup.name}</td>
                <td className="text-center">{getResource('Employee')}</td>
            </tr>
            {kpiGroup.details.map(this.renderKPI)}
        </Fragment>
    )

    getData = processId => {
        const data = {
            employeeId: this.state.employeeId,
            comment: null,
            evaluations: []
        };

        switch (processId) {
            case PerformanceProcess.Self:
                this.state.evaluationDetails.forEach(kpiGroup =>
                    data.evaluations.push(...kpiGroup.details.map(kpi => ({
                        employeeId: this.state.employeeId,
                        kpiId: kpiId,
                        point: kpi.selfPoint
                    })))
                );
                break;
            case PerformanceProcess.Evaluator:
                this.state.evaluationDetails.forEach(kpiGroup =>
                    data.evaluations.push(...kpiGroup.details.map(kpi => ({
                        employeeId: this.state.employeeId,
                        kpiId: kpiId,
                        point: kpi.evaluatorPoint
                    })))
                );
                break;
        }
    }

    handleSubmit = () => {
        const data = {
            employeeId: this.state.employeeId,
            comment: null,
            evaluations: []
        };

        this.state.evaluationDetails.forEach(kpiGroup =>
            data.evaluations.push(...kpiGroup.details.map(kpi => ({
                employeeId: this.state.employeeId,
                kpiId: kpiId,
                point: kpi.selfPoint
            })))
        );

        axios({
            url: resolveClientUrl('MyProbation/Edit'),
            method: 'post',
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                ...data
            }),
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(response => {
            const data = response.data;

            if (data.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));
            } else {
                toast.error(data.message);
            }
        }).catch(axiosError);
    }

    render() {
        if (this.state.probationer) {
            return (
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Probation Evaluation</li>
                    </Breadcrumb>

                    <div className="col-sm-12">
                        <NexPanel title={`PERFORMANCE REVIEW: SKILLS AND ABILITIE (${this.state.probationer.code} - ${this.state.probationer.fullName})`} >
                            <table className="table table-bordered" >
                                <tbody>
                                    <tr className="group-row">
                                        <td colSpan={6}>Please rate the performance using the following rating scale</td>
                                    </tr>
                                    <tr>
                                        <td className="text-center">{this.state.probationer.fullName}</td>
                                        <td className="text-center">Poor: 1 <i className="fa fa-star" /></td>
                                        <td className="text-center">Below Average: 2 <i className="fa fa-star" /><i className="fa fa-star" /></td>
                                        <td className="text-center">Sastisfactory: 3 <i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" /></td>
                                        <td className="text-center">Good: 4 <i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" /></td>
                                        <td className="text-center">Excellent:5  <i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" /></td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <img src={this.state.avatarUrl} alt="avatar" className="avatar" />
                                        </td>
                                        <td>
                                            Unsatisfactory: Performance was consistently below expectations. Poor quality of work, frequently needs rework. Significant improvement is needed.
                                        </td>
                                        <td>
                                            Improvement needed: Performance did not consistently meet expectations. Did not generally contribute extra effort or energy. An improvement plan is needed.
                                        </td>
                                        <td>
                                            Meets expectations: Solid performance with a good quality of work, at times possibly exceeding expectations and contributing additional value.
                                        </td>
                                        <td>
                                            Exceeds expectations: Significant performance and exceeded expectations. The quality of work overall was excellent and was able to deal with big problems with minimal direction.
                                        </td>
                                        <td>
                                            Exceptional: The most outstanding performance and far exceeded expectations due to exceptionally high quality of work. Made a superior contribution in support team/department objectives.
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </NexPanel>

                        <NexPanel title="For Employee Use">
                            <table className="table table-bordered table-striped">
                                <tbody>
                                    <tr className="group-row">
                                        <td colSpan="2">B. Quality of Work</td>
                                    </tr>
                                    {this.state.evaluationDetails.map(this.renderKPIGroup)}

                                    <tr>
                                        <td className="text-center score">Score</td>
                                        <td className="text-center">{this.state.probationer.selfPoint}</td>
                                    </tr>
                                </tbody>
                            </table>
                        </NexPanel>
                    </div >
                    <div className="col-sm-8" style={{ marginTop: '15px' }}>
                        <button className="btn btn-primary" onClick={this.handleSubmit}>Save</button>
                    </div>
                    <ToastLayout />
                </div >
            );
        }

        return <Fragment />;
    }
}

ReactDOM.render(<Probation />, document.getElementById('probation-wrapper'));