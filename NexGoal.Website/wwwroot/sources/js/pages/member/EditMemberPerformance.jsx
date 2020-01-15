import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import { Button, ControlLabel, Modal, FormGroup } from 'react-bootstrap';
import Select from 'react-select';
import axios from 'axios';
import { toast } from 'react-toastify';
import { isEmpty } from 'lodash';
import { stringify } from 'qs';
import moment from 'moment';
import isEqual from 'react-fast-compare';

import { axiosError, getRoleClassName, resolveClientUrl } from '../../common-functions';
import { DateFormat, ProbationTag, PostRequestHeaders } from '../../constants';
import getResource from '../../resourceHelper';
import TextareaFullscreenEditor from '../../shared/components/textarea-fullscreen/index';
import PerformanceGradeInfo from '../../shared/components/PerformanceGradeInfo';
import { validateCreateEditMemberPerformance, axiosEditPerformanceResponse } from '../home/common-functions';

export default class EditMemberPerformance extends Component {
    static propTypes = {
        currentUser: PropTypes.objectOf(PropTypes.object),
        performanceRecordId: PropTypes.number,
        memberOptions: PropTypes.arrayOf(PropTypes.object),
        performanceGradeOptions: PropTypes.arrayOf(PropTypes.object),
        projectOptions: PropTypes.arrayOf(PropTypes.object),
        isShowModal: PropTypes.bool,
        onCloseModal: PropTypes.func,
        onSubmitSuccess: PropTypes.func,
        isDraft: PropTypes.bool
    };

    static defaultProps = {
        currentUser: { nickName: '', avatar: '' },
        performanceRecordId: 0,
        memberOptions: [],
        performanceGradeOptions: [],
        projectOptions: [],
        isDraft: false,
        isShowModal: false,
        onCloseModal: function () {
            //default: do nothing
        },
        onSubmitSuccess: function () {
            //default: do nothing
        }
    };

    constructor(props) {
        super(props);

        this.state = {
            performanceRecord: {},
            initialPerformanceRecord: {},
            errors: {},
            isSubmitting: false,
            showGradeInfo: false
        };
    }

    componentDidMount() {
        const url = this.props.isDraft ? `Performance/GetDraftById/${this.props.performanceRecordId}` : `Performance/GetById/${this.props.performanceRecordId}`;
        axios(resolveClientUrl(url))
            .then(response => {
                const performanceRecord = response.data;
                performanceRecord.grade = performanceRecord.grade ? this.props.performanceGradeOptions.find(x => x.value === performanceRecord.grade.gradeId) : null;

                performanceRecord.employees = performanceRecord.employees.map(employee => {
                    return { value: employee.employeeId, label: `${employee.nickName} (${employee.fullName})` };
                });
                performanceRecord.projects = performanceRecord.projects.map(project => {
                    return { value: project.projectId, label: project.name };
                });

                this.setState({ performanceRecord: performanceRecord, initialPerformanceRecord: { ...performanceRecord } });
            }).catch(axiosError);

        window.addEventListener('beforeunload', this.onLeavingPageEventListener);
    }

    componentWillUnmount() {
        window.removeEventListener('beforeunload', this.onLeavingPageEventListener);
    }

    onLeavingPageEventListener = event => {
        let { performanceRecord, initialPerformanceRecord } = this.state;

        if (!isEqual(performanceRecord, initialPerformanceRecord)) {
            event.returnValue = ' ';
            return ' ';
        }

        return null;
    }

    handleEmployeeChange = selectedOptions => {
        const projects = [...this.state.performanceRecord.projects];
        const index = projects.length > 0 ? projects.findIndex(item => item.value === ProbationTag.value) : -1;
        const errors = { ...this.state.errors };
        delete errors['employee'];

        if (selectedOptions.length === 0) {
            errors.employee = getResource('EmployeeRequired');
        }

        if (selectedOptions.length === 1 && selectedOptions[0].isProbation && index < 0) {
            projects.push(ProbationTag);
        }

        if (selectedOptions.length === 0 || selectedOptions.length > 1) {
            projects.splice(index, 1);
        }

        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { employees: selectedOptions, projects: projects }),
            errors: errors
        });
    }

    handleGradeChange = selectedOption => {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { grade: selectedOption })
        });
    }

    handleCommentChange = event => {
        const errors = { ...this.state.errors };
        delete errors['comment'];
        if (!event.target.value) {
            errors.comment = getResource('CommentRequired');
        }

        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { comment: event.target.value }),
            errors: errors
        });
    }

    handleProjectChange = selectedOptions => {
        this.setState({
            performanceRecord: Object.assign({}, this.state.performanceRecord, { projects: selectedOptions })
        });
    }

    handleSubmit = event => {
        event.preventDefault();

        if (this.state.isSubmitting) {
            return;
        }

        const isValid = validateCreateEditMemberPerformance(this);
        if (isValid) {
            this.setSubmitting(true);

            const record = this.state.performanceRecord;

            const performanceRecord = stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                performanceRecordId: record.performanceRecordId,
                employeeIds: record.employees.map(employee => employee.value).join(','),
                gradeId: record.grade ? record.grade.value : null,
                comment: record.comment.trim(),
                isFromDraft: record.isDraft,
                projectIds: record.projects.map(project => project.value).join(',')
            });

            axios({
                url: resolveClientUrl('Member/Performance/Edit'),
                method: 'POST',
                data: performanceRecord,
                headers: PostRequestHeaders
            }).then(response => {
                axiosEditPerformanceResponse(this, toast, response, () => {
                    this.props.onSubmitSuccess(response.data.objectIds);
                });
            }).catch(axiosError).then(() => this.setSubmitting(false));
        }
    }

    handleSaveAsDraft = event => {
        event.preventDefault();

        if (this.state.isSubmitting) {
            return;
        }

        const isValid = validateCreateEditMemberPerformance(this);
        if (isValid) {
            this.setSubmitting(true);

            const record = this.state.performanceRecord;

            const performanceRecord = stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                performanceRecordId: record.performanceRecordId,
                employeeIds: record.employees.map(employee => employee.value).join(','),
                gradeId: record.grade ? record.grade.value : null,
                comment: record.comment.trim(),
                projectIds: record.projects.map(project => project.value).join(',')
            });

            axios({
                url: resolveClientUrl('Member/Performance/EditDraft'),
                method: 'POST',
                data: performanceRecord,
                headers: PostRequestHeaders
            }).then(response => {
                axiosEditPerformanceResponse(this, toast, response, () => {
                    this.props.onSubmitDraftSuccess(record.performanceRecordId);
                });
            }).catch(axiosError).then(() => this.setSubmitting(false));
        }
    }

    handleOpenGradeInfo = () => this.setState({ showGradeInfo: true })

    handleCloseGradeInfo = () => this.setState({ showGradeInfo: false })

    setSubmitting = isSubmitting => this.setState({ isSubmitting })

    render() {
        const { performanceRecord, errors } = this.state;
        const { memberOptions, performanceGradeOptions, projectOptions, currentUser } = this.props;

        return (
            <Fragment>
                <Modal keyboard={false} backdrop="static"
                    show={this.props.isShowModal} onHide={this.props.onCloseModal}
                    dialogClassName="custom-modal modal-md form-performance edit "
                >
                    <Modal.Header>
                        <Modal.Title>{getResource('EditPerformanceCheckin')}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <div className="record record-your-mind">
                            <div className="avatar-user">
                                <img src={currentUser.avatar} className="avatar" />
                                <div className="person-info">
                                    <span className={getRoleClassName(performanceRecord.roleId)}>{currentUser.nickName}</span>
                                </div>
                            </div>
                            <div className="date-time">
                                {
                                    performanceRecord.isDraft &&
                                    <span className="draft">draft</span>
                                }
                                <span className="submit-date">{moment().format(DateFormat)}</span>
                            </div>
                            <div className="comment">
                                <form className="form-filter form-performance edit">
                                    <FormGroup>
                                        <ControlLabel>{getResource('Member')}</ControlLabel>
                                        <Select name="employees" isMulti closeMenuOnSelect={false} classNamePrefix="react-select"
                                            className={`react-select-container employee-field ${!isEmpty(errors) && errors.employee ? 'input-validation-error' : ''}`}
                                            placeholder="Select Employees"
                                            value={performanceRecord.employees} options={memberOptions}
                                            onChange={this.handleEmployeeChange} isClearable
                                            isDisabled={this.props.isDisabledAddMember || !performanceRecord.isDraft}
                                        />
                                    </FormGroup>
                                    <FormGroup>
                                        <ControlLabel>{getResource('Comment')}</ControlLabel>
                                        <TextareaFullscreenEditor
                                            className={`form-control comment-field ${!isEmpty(errors) && errors.comment ? 'input-validation-error' : ''}`}
                                            name="comment"
                                            value={performanceRecord.comment}
                                            onChange={this.handleCommentChange}
                                        />
                                    </FormGroup>
                                    <FormGroup>
                                        <div className="grade">
                                            <ControlLabel>{getResource('Grade')} <a className="grade-info" onClick={this.handleOpenGradeInfo}></a></ControlLabel>
                                            <Select name="grade" className="react-select-container grade-field" classNamePrefix="react-select"
                                                placeholder="Select Grade"
                                                value={performanceRecord.grade}
                                                options={performanceGradeOptions}
                                                isSearchable={false}
                                                isClearable
                                                onChange={this.handleGradeChange}
                                            />
                                        </div>
                                        <div className="project">
                                            <ControlLabel>{getResource('ProjectTag')}</ControlLabel>
                                            <Select name="projects" isMulti closeMenuOnSelect={false} placeholder="Projects"
                                                className="react-select-container project-field" classNamePrefix="react-select"
                                                value={performanceRecord.projects}
                                                options={projectOptions}
                                                onChange={this.handleProjectChange}
                                            />
                                        </div>
                                    </FormGroup>
                                </form>
                            </div>
                        </div>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Cancel')}</Button>
                        {
                            performanceRecord.isDraft &&
                            <Button type="submit" className="btn-cancel" name="saveAsDraft"
                                onClick={this.handleSaveAsDraft} disabled={this.state.isSubmitting}
                            >
                                {getResource('SaveAsDraft')}
                            </Button>
                        }
                        <Button type="submit" bsStyle="primary" name="save"
                            onClick={this.handleSubmit} disabled={this.state.isSubmitting}
                        >
                            {getResource('Publish')}
                        </Button>
                    </Modal.Footer>
                </Modal>
                <PerformanceGradeInfo showModal={this.state.showGradeInfo} performanceGradeInfos={this.props.performanceGradeInfos} onCloseModal={this.handleCloseGradeInfo} />
            </Fragment>

        );
    }
}