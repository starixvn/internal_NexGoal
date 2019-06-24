import React, { Component, Fragment } from 'react';

import Select from 'react-select';
import axios from 'axios';
import * as Yup from 'yup';
import Rating from 'react-rating';
import { flatMap, round, sumBy } from 'lodash';
import { Formik, getIn } from 'formik';
import { stringify } from 'qs';
import { toast } from 'react-toastify';
import { axiosError, resolveClientUrl } from '../../../common-functions';
import getResource from '../../../resourceHelper';
import ProbationResultOptions from './ProbationResultOptions';
import SingleValue from './SingleValue';
import { error } from 'util';

export default class EvaluatorForm extends Component {
    constructor(props) {
        super(props);
    }

    updatePoint = (kpi, value) => {
        const evaluationDetails = this.state.evaluationDetails.slice();
        kpi.evaluatorPoint = value * 1.11;

        this.setState({ evaluationDetails });
        this.calcuateAverageGrade();
    }

    emptySymbol = <i className="rating-icon fa fa-star-o" />

    fullSymbol = <i className="rating-icon fa fa-star" />

    handleSelfCommentChange = (event) => {
        const probationer = this.props.probationer;
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

    schema = Yup.object().shape({
        comment: Yup.string().required(),
        evaluations: Yup.array().of(
            Yup.object().shape({
                details: Yup.array().of(
                    Yup.object().shape({
                        evaluatorPoint: Yup.number().required()
                    })
                )
            })
        ),
        isQualifiedByEvaluator: Yup.object().required()
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
                <table className="table table-bordered table-striped">
                    <tbody>
                        <tr className="group-row">
                            <td colSpan="3">B. Quality of Work</td>
                        </tr>
                        {values.evaluations.map((kpiGroup, gIndex) => (
                            <Fragment key={kpiGroup.kpiGroupId}>
                                <tr className="group-row">
                                    <td>{gIndex + 1}. {kpiGroup.name}</td>
                                    <td className="text-center">{getResource('Employee')}</td>
                                    <td className="text-center">{getResource('Evaluator')}</td>
                                </tr>
                                {kpiGroup.details.map((kpi, index) => {
                                    score += kpi.evaluatorPoint;
                                    return (
                                        <tr key={kpi.kpiId}>
                                            <td>{index + 1}. {kpi.kpiName}</td>
                                            <td className="text-center">{kpi.selfPoint}</td>
                                            <td className="text-center">
                                                {this.props.probationer.canEdit &&
                                                    <Rating initialRating={kpi.evaluatorPoint / 1.11 || 0}
                                                        emptySymbol={this.emptySymbol}
                                                        fullSymbol={this.fullSymbol}
                                                        onChange={value => setFieldValue(`evaluations.${gIndex}.details.${index}.evaluatorPoint`, round(value * 1.11, 2))}
                                                        className={`rating ${getIn(touched, `evaluations.${gIndex}.details.${index}.evaluatorPoint`)
                                                            && getIn(errors, `evaluations.${gIndex}.details.${index}.evaluatorPoint`)
                                                            && 'input-validation-error'}`}
                                                    />
                                                }
                                                {kpi.evaluatorPoint && `${kpi.evaluatorPoint} pts`}
                                            </td>
                                        </tr>
                                    )
                                })}
                            </Fragment>
                        ))}

                        <tr>
                            <td className="text-center score">Score</td>
                            <td className="text-center">{this.props.probationer.selfScore && `${this.props.probationer.selfScore}/100`}</td>
                            <td className="text-center">{score > 0 && `${round(score)}/100`}</td>
                        </tr>  <tr>
                            <td className="text-center score" colSpan="2">{getResource('Result')}</td>
                            <td>
                                <Select name="isQualifiedByEvaluator"
                                    className={`react-select-container ${touched.isQualifiedByEvaluator && errors.isQualifiedByEvaluator && 'input-validation-error'}`}
                                    classNamePrefix="react-select" isDisabled={!this.props.probationer.canEdit}
                                    value={values.isQualifiedByEvaluator}
                                    options={ProbationResultOptions} placeholder=""
                                    isClearable={false} isSearchable={false}
                                    components={{ SingleValue }}
                                    onChange={value => setFieldValue('isQualifiedByEvaluator', value)}
                                />
                            </td>
                        </tr>
                        <tr>
                            <td colSpan="3">Employee's Comment</td>
                        </tr>
                        <tr>
                            <td colSpan="3" className="comment-cell">
                                <p>{this.props.probationer.selfComment}</p>
                            </td>
                        </tr>
                        <tr>
                            <td colSpan="3">Evaluator's Comment</td>
                        </tr>
                        <tr>
                            <td colSpan="3" className="comment-cell">
                                <div>
                                    <textarea className={`${touched.comment && errors.comment && 'input-validation-error'}`}
                                        value={values.comment} onChange={handleChange} name="comment" />
                                </div>

                            </td>
                        </tr>
                    </tbody>
                </table>
                <div style={{ marginTop: '15px' }}>
                    <a className="btn btn-default btn-cancel" href="/member/index?activeTab=4">{getResource('Back')}</a>
                    {this.props.probationer.canEdit &&
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
            point: kpi.evaluatorPoint
        })));

        const data = {
            employeeId: this.props.probationer.employeeId,
            comment: values.comment,
            evaluations: evaluations,
            totalScore: round(sumBy(evaluations, 'point')),
            isQualifiedByEvaluator: values.isQualifiedByEvaluator.value
        };

        axios({
            url: resolveClientUrl('Probation/EditEvaluatorEvaluation'),
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
        return (
            <Formik
                initialValues={{
                    comment: this.props.probationer.evaluatorComment,
                    evaluations: this.props.evaluationDetails,
                    isQualifiedByEvaluator: this.props.probationer.isQualifiedByEvaluator ? ProbationResultOptions.find(x => x.value === this.props.probationer.isQualifiedByEvaluator) : null
                }}
                render={this.renderForm}
                validationSchema={this.schema}
                onSubmit={this.handleSubmit}
            />
        );
    }
}