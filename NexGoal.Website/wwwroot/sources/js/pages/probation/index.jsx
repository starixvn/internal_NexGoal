import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';

import axios from 'axios';
import { stringify } from 'qs';
import Rating from 'react-rating';
import { toast } from 'react-toastify';
import moment from 'moment';
import { flatMap, round, sumBy } from 'lodash';
import { Formik, getIn } from 'formik';
import { OverlayTrigger } from 'react-bootstrap';

import NexPanel from '../../shared/components/NexPanel';
import { axiosError, resolveClientUrl } from '../../common-functions';
import { DateFormat, PerformanceProcess, PostRequestHeaders } from '../../constants';
import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import ProbationProcess from './ProbationProcess';
import HRM from './HRM';
import PerformanceAppraisalResult from '../home/PerformanceAppraisalResult';
import Description from './Description';
import EvaluationDetailTooltip from './EvaluationDetailTooltip';
import { emptySymbol, fullSymbol } from './RatingSymbol';

export default class MyProbation extends Component {
    constructor(props) {
        super(props);

        this.state = {
            employeeInfo: {
                nickName: document.getElementById('employeeInfo-nickName').innerText,
                fullName: document.getElementById('employeeInfo-fullName').innerText,
                jobTitle: document.getElementById('employeeInfo-jobTitle').innerText,
                avatar: document.getElementById('employeeInfo-avatar').getAttribute('src'),
                groupName: document.getElementById('employeeInfo-groupname').value,
                joinedDate: moment(document.getElementById('employeeInfo-joineddate').value),
                isProbation: document.getElementById('employeeInfo-isProbation').checked,
                code: document.getElementById('employeeInfo-code').value
            },
            probationer: null,
            evaluationDetails: []
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('MyProbation/GetEvaluationDetail')),
            axios(resolveClientUrl('MyProbation/GetEvaluation')),
            axios(resolveClientUrl('MyProbation/GetHRM')),
            axios(resolveClientUrl('home/GetPerformanceAppraisalResult'))
        ];

        Promise.all(promises)
            .then(([evaluationDetailResponse, probationerResponse, hrmResponse, performanceAppraisalsResponse]) => {
                const probationer = probationerResponse.data;

                probationer.settings.forEach(setting => {
                    setting.fromDate = moment(setting.fromDate);
                    setting.toDate = moment(setting.toDate);
                });

                const selfProcess = probationer.settings.find(x => x.processId = PerformanceProcess.Self);

                const today = moment().format('YYYY-MMM-DD');
                const fromDate = moment(selfProcess.fromDate).format('YYYY-MMM-DD');

                this.setState({
                    evaluationDetails: evaluationDetailResponse.data,
                    probationer: probationer,
                    hrm: hrmResponse.data,
                    performanceAppraisals: performanceAppraisalsResponse.data,
                    selfProcess: selfProcess,
                    showEvaluationForm: moment(today).isSameOrAfter(fromDate)
                });
            }).catch(axiosError);
    }

    evaluationDetailTitle = <Fragment>
        Quality of Work
        <OverlayTrigger placement="bottom" overlay={EvaluationDetailTooltip}>
            <i className="icon-info" />
        </OverlayTrigger>
    </Fragment>;

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        setFieldError,
        isSubmitting
    }) => {
        let score = 0;

        return (
            <Fragment>
                <NexPanel title={this.evaluationDetailTitle} enable={false}>
                    <table className="table table-bordered table-striped">
                        <tbody>
                            {values.evaluations.map((kpiGroup, gIndex) => (
                                <Fragment key={kpiGroup.kpiGroupId}>
                                    <tr className="group-row">
                                        <td className="text-uppercase">{gIndex + 1}. {kpiGroup.name}</td>
                                        <td className="text-center">{getResource('Employee')}</td>
                                    </tr>
                                    {kpiGroup.details.map((kpi, index) => {
                                        score += kpi.selfPoint;
                                        return (
                                            <tr key={kpi.kpiId}>
                                                <td>{gIndex + 1}-{index + 1}. {kpi.kpiName}</td>
                                                <td className="text-center">
                                                    {this.state.probationer.canEdit &&
                                                        <Rating initialRating={kpi.selfPoint / 1.11 || 0}
                                                            emptySymbol={emptySymbol}
                                                            fullSymbol={fullSymbol}
                                                            onChange={value => {
                                                                setFieldValue(`evaluations.${gIndex}.details.${index}.selfPoint`, round(value * 1.11, 2));
                                                                setFieldError(`evaluations.${gIndex}.details.${index}.selfPoint`, null);
                                                            }}
                                                            className={`rating ${getIn(touched, `evaluations.${gIndex}.details.${index}.selfPoint`)
                                                                && getIn(errors, `evaluations.${gIndex}.details.${index}.selfPoint`)
                                                                && 'input-validation-error'}`}
                                                        />
                                                    }
                                                    <strong>{kpi.selfPoint}</strong>
                                                </td>
                                            </tr>
                                        );
                                    })}
                                </Fragment>
                            ))}

                            <tr className="score">
                                <td className="text-center text-bold">Score</td>
                                <td className="text-center">
                                    {score > 0 &&
                                        <span className="score-badge"><span>{round(score)}</span> / 100 pts</span>
                                    }
                                </td>
                            </tr>
                            <tr>
                                <td className="comment-title" colSpan={2}>Employee's Comment</td>
                            </tr>
                            <tr>
                                <td colSpan={2} className="comment-cell">
                                    <textarea className={touched.comment && errors.comment && 'input-validation-error'}
                                        name="comment" rows="7"
                                        readOnly={!this.state.probationer.canEdit}
                                        value={values.comment}
                                        onChange={event => {
                                            handleChange(event);
                                            setFieldError(`comment`, null);
                                        }}
                                    />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </NexPanel>
                <div className="btn-group">
                    {this.state.probationer.canEdit &&
                        <button className="btn btn-primary" disabled={isSubmitting} type="button" onClick={handleSubmit}>{getResource('Submit')}</button>
                    }
                </div>
            </Fragment>
        );
    };

    handleSubmit = (values, { setSubmitting }) => {
        const evaluations = flatMap(values.evaluations, kpiGroup => kpiGroup.details.map(kpi => ({
            employeeId: kpi.employeeId,
            kpiId: kpi.kpiId,
            point: kpi.selfPoint
        })));

        const data = {
            comment: values.comment,
            evaluations: evaluations,
            totalScore: round(sumBy(evaluations, 'point'))
        };

        axios({
            url: resolveClientUrl('MyProbation/EditSelfEvaluation'),
            method: 'post',
            headers: PostRequestHeaders,
            data: stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                ...data
            })
        }).then(res => {
            const result = res.data;
            if (result.isSuccessful) {
                toast.success(getResource('UpdatedSuccessfully'));
            } else {
                toast.error(result.message);
            }

            setSubmitting(false);
        }).catch(axiosError);
    };

    validateData = values => {
        let errors = {};
        let isValid = true;

        if (!values.comment) {
            errors.comment = 'Required';
            isValid = false;
        }

        values.evaluations.forEach((eva, indexEva) => {
            if (!errors.evaluations) {
                errors.evaluations = [];
            }
            errors.evaluations[indexEva] = {};

            eva.details.forEach((kpi, indexKPI) => {
                if (!errors.evaluations[indexEva].details) {
                    errors.evaluations[indexEva].details = [];
                }
                errors.evaluations[indexEva].details[indexKPI] = {};
                if (!kpi.selfPoint) {
                    isValid = false;
                    errors.evaluations[indexEva].details[indexKPI].selfPoint = 'Required';
                }
            })
        })

        if (!isValid) {
            toast.error('Please input all required fields.');
        } else {
            errors = {};
        }

        return errors;
    };

    render() {
        if (this.state.probationer) {
            return (
                <div className="row">
                    <div className="main-layout">
                        <div className="general-info">
                            <div className="user-info box">
                                <img src={this.state.employeeInfo.avatar} alt="avatar" className="avatar" />
                                <div>
                                    <div className="nick-name role-member">{this.state.employeeInfo.nickName}</div>
                                    <div className="user-name">{this.state.employeeInfo.fullName}</div>
                                    <div className="role role-PM">{this.state.employeeInfo.jobTitle}</div>
                                </div>
                            </div>
                            <PerformanceAppraisalResult performanceAppraisals={this.state.performanceAppraisals} />
                        </div>
                        <div className="probation-content">
                            {this.state.showEvaluationForm ?
                                <Fragment>
                                    <NexPanel title="PERFORMANCE REVIEW: SKILLS AND ABILITIE">
                                        <Description employeeInfo={this.state.employeeInfo} />
                                        <ProbationProcess processes={this.state.probationer.settings} />
                                    </NexPanel>

                                    <NexPanel title="HRM System" enable={false}>
                                        <HRM hrm={this.state.hrm} />
                                    </NexPanel>
                                    <Formik
                                        initialValues={{
                                            comment: this.state.probationer.selfComment,
                                            evaluations: this.state.evaluationDetails
                                        }}
                                        render={this.renderForm}
                                        validateOnChange={false}
                                        validate={this.validateData}
                                        onSubmit={this.handleSubmit}
                                    />
                                </Fragment>
                                :
                                <div className="announcement">
                                    <div className="title">{getResource('Announcement')}</div>
                                    <div className="info">
                                        Welcome to your probation self-evaluation.
                                        Please be informed that this form will not be available until {this.state.selfProcess.fromDate.format(DateFormat)}.
                                        Thank you for your concern.
                                    </div>
                                </div>
                            }
                        </div >

                        <ToastLayout />
                    </div >
                </div>
            );
        }

        return <Fragment />;
    }
}

ReactDOM.render(<MyProbation />, document.getElementById('probation-wrapper'));