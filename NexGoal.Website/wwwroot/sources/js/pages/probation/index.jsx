import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';

import axios from 'axios';
import Select from 'react-select';
import { stringify } from 'qs';
import { set } from 'lodash';
import Rating from 'react-rating';
import { toast } from 'react-toastify';
import moment from 'moment';
import * as Yup from 'yup';
import { flatMap, round, sumBy } from 'lodash';
import { Formik, getIn } from 'formik';

import NexPanel from '../../shared/components/NexPanel';
import { axiosError, resolveClientUrl } from '../../common-functions';
import getResource from '../../resourceHelper';
import { PerformanceProcess } from '../../constants';
import ToastLayout from '../../shared/layouts/ToastLayout';
import ProbationProcess from './ProbationProcess';
import HRM from './HRM';
import PerformanceAppraisalResult from './PerformanceAppraisalResult';

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
                isProbation: document.getElementById('employeeInfo-isProbation').checked
            },
            probationer: null,
            evaluationDetails: [],
            avatarUrl: document.getElementById('employeeInfo-avatar').getAttribute('src')
        };
    }

    componentDidMount() {
        const promises = [
            axios(resolveClientUrl('MyProbation/GetEvaluationDetail')),
            axios(resolveClientUrl('MyProbation/GetEvaluation')),
            axios(resolveClientUrl('MyProbation/GetHRM')),
            axios(resolveClientUrl('home/GetPerformanceAppraisalResult')),

        ];

        Promise.all(promises)
            .then(([evaluationDetailResponse, probationerResponse, hrmResponse, performanceAppraisalsResponse]) => {
                const probationer = probationerResponse.data;
                probationer.settings.forEach(setting => {
                    setting.fromDate = moment(setting.fromDate);
                    setting.toDate = moment(setting.toDate);
                });
                this.setState({
                    evaluationDetails: evaluationDetailResponse.data,
                    probationer: probationer,
                    hrm: hrmResponse.data,
                    performanceAppraisals: performanceAppraisalsResponse.data,
                });
            }).catch(axiosError);
    }

    updatePoint = (kpi, value) => {
        const evaluationDetails = this.state.evaluationDetails.slice();
        kpi.selfPoint = value * 1.11;

        this.setState({ evaluationDetails });
        this.calcuateAverageGrade();
    }

    emptySymbol = <i className="rating-icon fa fa-star grey" />

    fullSymbol = <i className="rating-icon fa fa-star" />

    renderKPI = (kpi, index) => (
        <tr key={kpi.kpiId}>
            <td>{index + 1}. {kpi.kpiName}</td>
            <td className="text-center">
                {this.state.probationer.canEdit &&
                    <Rating initialRating={kpi.selfPoint / 1.11 || 0}
                        emptySymbol={this.emptySymbol}
                        fullSymbol={this.fullSymbol}
                        onChange={value => this.updatePoint(kpi, value)}
                    />
                }
                {kpi.selfPoint && `${kpi.selfPoint} pts`}

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

    handleSelfCommentChange = (event) => {
        const probationer = this.state.probationer;
        probationer.selfComment = event.target.value;

        if (!this.state.error) {
            this.setState({ probationer });
        }
        else {
            this.setState(prevState => ({
                probationer: probationer,
                error: Object.assign({}, prevState.error, { comment: event.target.value ? null : getResource('CommentRequired') })
            }));
        }
    }

    calcuateAverageGrade = () => {
        let sum = 0;
        console.log(flatMap(this.state.evaluationDetails, (kpiGroup) => kpiGroup.details.map(kpi => ({
            employeeId: kpi.employeeId,
            kpiId: kpi.kpiId,
            point: kpi.selfPoint
        }))));
    }

    schema = Yup.object().shape({
        comment: Yup.string().required(),
        evaluations: Yup.array().of(
            Yup.object().shape({
                details: Yup.array().of(
                    Yup.object().shape({
                        selfPoint: Yup.number().required()
                    })
                )
            })
        )
    })

    renderForm = ({
        values,
        errors,
        touched,
        handleChange,
        handleSubmit,
        setFieldValue,
        isSubmitting
    }) => {
        let score = 0;

        return (
            <Fragment>
                <NexPanel title="B. Quality of Work">
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
                                                <td>{index + 1}. {kpi.kpiName}</td>
                                                <td className="text-center">
                                                    {this.state.probationer.canEdit &&
                                                        <Rating initialRating={kpi.selfPoint / 1.11 || 0}
                                                            emptySymbol={this.emptySymbol}
                                                            fullSymbol={this.fullSymbol}
                                                            onChange={value => setFieldValue(`evaluations.${gIndex}.details.${index}.selfPoint`, round(value * 1.11, 2))}
                                                            className={`rating ${getIn(touched, `evaluations.${gIndex}.details.${index}.selfPoint`)
                                                                && getIn(errors, `evaluations.${gIndex}.details.${index}.selfPoint`)
                                                                && 'input-validation-error'}`}
                                                        />
                                                    }
                                                    <strong>{kpi.selfPoint && `${kpi.selfPoint} pts`}</strong>
                                                </td>
                                            </tr>
                                        )
                                    })}
                                </Fragment>
                            ))}

                            <tr>
                                <td className="text-center score text-bold">Score</td>
                                <td className="text-center score"><span className="score-badge">{score > 0 && `${round(score)}/100`}</span></td>
                            </tr>
                            <tr>
                                <td colSpan={2}>Employee's Comment</td>
                            </tr>
                            <tr>
                                <td colSpan={2} className="comment-cell">
                                    <textarea className={`none-resize ${touched.comment && errors.comment && 'input-validation-error'}`}
                                        value={values.comment} onChange={handleChange} name="comment" />
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </NexPanel>
                <div style={{ marginTop: '15px' }}>
                    {this.state.probationer.canEdit &&
                        <button className="btn btn-primary" disabled={isSubmitting} type="button" onClick={handleSubmit}>Save</button>
                    }
                </div>
            </Fragment>
        );
    }

    handleSubmit = (values, { setErrors, setSubmitting }) => {
        const evaluations = flatMap(values.evaluations, (kpiGroup) => kpiGroup.details.map(kpi => ({
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

            setSubmitting(false);
        }).catch(axiosError);
    }

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
                            <NexPanel title={`PERFORMANCE REVIEW: SKILLS AND ABILITIE (${this.state.probationer.code} - ${this.state.probationer.fullName})`} >
                                <div className="info-rate">
                                    <div className="user-info">
                                        <img src={this.state.avatarUrl} alt="avatar" className="avatar" />
                                        <ul>
                                            <li className="info">
                                                <span className="label">Full Name:</span>
                                                <span className="info-text">Nguyễn Văn A</span>
                                            </li>
                                            <li className="info">
                                                <span className="label">Nick Name:</span>
                                                <span className="info-text">Nick</span>
                                            </li>
                                            <li className="info">
                                                <span className="label">Joined Day:</span>
                                                <span className="info-text">01 May, 2019</span>
                                            </li>
                                            <li className="info">
                                                <span className="label">Team:</span>
                                                <span className="info-text">DEV Pool</span>
                                            </li>
                                            <li className="info">
                                                <span className="label">Job Title:</span>
                                                <span className="info-text">Software Engineer</span>
                                            </li>
                                        </ul>
                                    </div>

                                    <div className="rate-information">
                                        <div className="rate-item">
                                            <div className="rate-star">
                                                <span>Poor: 1 </span>
                                                <div className="star-block">
                                                    <i className="fa fa-star" />
                                                </div>                                                
                                            </div>
                                            <div className="rate-description">Unsatisfactory: Performance was consistently below expectations. Poor quality of work, frequently needs rework. Significant improvement is needed.</div>
                                        </div>
                                        <div className="rate-item">
                                            <div className="rate-star">
                                                <span>Below Average: 2</span>
                                                <div className="star-block">
                                                    <i className="fa fa-star" /><i className="fa fa-star" />
                                                </div>                                                
                                            </div>
                                            <div className="rate-description">Improvement needed: Performance did not consistently meet expectations. Did not generally contribute extra effort or energy. An improvement plan is needed.</div>
                                        </div>                                        
                                        <div className="rate-item">
                                            <div className="rate-star">
                                                <span>Sastisfactory: 3</span>
                                                <div className="star-block">
                                                    <i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" />
                                                </div>                                                
                                            </div>
                                            <div className="rate-description">Meets expectations: Solid performance with a good quality of work, at times possibly exceeding expectations and contributing additional value.</div>
                                        </div>
                                        <div className="rate-item">
                                            <div className="rate-star">
                                                <span>Good: 4</span>
                                                <div className="star-block">
                                                    <i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" />
                                                </div>
                                            </div>
                                            <div className="rate-description">Exceeds expectations: Significant performance and exceeded expectations. The quality of work overall was excellent and was able to deal with big problems with minimal direction.</div>
                                        </div>
                                        <div className="rate-item">
                                            <div className="rate-star">
                                                <span>Excellent:5</span>
                                                <div className="star-block">
                                                    <i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" /><i className="fa fa-star" />
                                                </div>
                                            </div>
                                            <div className="rate-description">Exceptional: The most outstanding performance and far exceeded expectations due to exceptionally high quality of work. Made a superior contribution in support team/department objectives.</div>
                                        </div>
                                    </div>                                    
                                </div>

                                {/*<table className="table table-bordered rate-info-table" >
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
                            </table>*/}

                            <ProbationProcess processes={this.state.probationer.settings} />
                        </NexPanel>

                        

                        <NexPanel title="A. HRM System">
                            <HRM hrm={this.state.hrm} />
                        </NexPanel>
                        <Formik
                            initialValues={{
                                comment: this.state.probationer.selfComment,
                                evaluations: this.state.evaluationDetails,
                                score: this.state.probationer.selfScore
                            }}
                            render={this.renderForm}
                            validationSchema={this.schema}
                            onSubmit={this.handleSubmit}
                        />

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