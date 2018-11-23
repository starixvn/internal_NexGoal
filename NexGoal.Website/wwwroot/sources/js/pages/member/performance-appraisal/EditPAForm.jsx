import React, { Component } from 'react';
import { Button, ControlLabel, Modal, FormGroup, Tab, Tabs } from 'react-bootstrap';
import Select from 'react-select';
import { camelCase, isEmpty } from 'lodash';
import { toast } from 'react-toastify';
import { stringify } from 'qs';
import TextareaFullscreenEditor from '../../../shared/components/textarea-fullscreen/index';
import getResource from '../../../resourceHelper';
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../../common-functions';
import { PerformanceProcess } from '../../../constants';
import PerformanceRecord from '../../../shared/components/PerformanceRecord';

export default class EditMemberPA extends Component {
    constructor(props) {
        super(props);
        this.state = {
            paRecord: {
                paRecordId: null,
                grade: null,
                comment: ''
            },
            paProcesses: [],
            errors: {},
            activeTabKey: 1
        };
    }

    componentDidMount() {
        axios(resolveClientUrl(`Review/GetPARecordById/${this.props.paRecordId}`)).then(response => {
            if (response.data) {
                let pa = response.data;
                this.setState({
                    paRecord: {
                        paRecordId: pa.paRecordId,
                        grade: pa.grade ?
                            this.props.gradeOptions.find(o => o.value === pa.grade.gradeId) : null,
                        comment: pa.comment
                    },
                    paProcesses: pa.processes
                });
            }
        }).catch(axiosError);
    }

    handleGradeChange = (selectedOption) => {
        this.setState({
            paRecord: Object.assign({}, this.state.paRecord, { grade: selectedOption })
        });
    }

    handleCommentChange = (event) => {
        let errors = { ...this.state.errors };
        delete errors['comment'];
        if (!event.target.value) {
            errors.comment = getResource('CommentRequired');
        }

        this.setState({
            paRecord: Object.assign({}, this.state.paRecord, { comment: event.target.value }),
            errors: errors
        });
    }

    validateForm = () => {
        let errors = {}, isValid = true;
        let paRecord = this.state.paRecord;

        if (!paRecord.comment || paRecord.comment.trim() === '') {
            errors.comment = getResource('CommentRequired');
            isValid = false;
        }

        if (!paRecord.grade || paRecord.grade.value <= 0) {
            errors.grade = getResource('GradeRequired');
            isValid = false;
        }

        this.setState({ errors });

        return isValid;
    }

    handleSubmit = (event) => {
        event.preventDefault();

        if (this.validateForm()) {
            let record = this.state.paRecord;

            let paRecord = stringify({
                __RequestVerificationToken: document.getElementsByName('__RequestVerificationToken')[0].value,
                paRecordId: record.paRecordId,
                gradeId: record.grade ? record.grade.value : null,
                comment: record.comment
            });
            axios({
                url: resolveClientUrl('Review/Edit'),
                method: 'POST',
                data: paRecord,
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }).then(response => {
                let data = response.data;

                if (data.isSuccessful) {
                    toast.success(getResource('CreatedSuccessfully'));

                    this.props.onSubmitSuccess();
                } else {
                    if (data.errors) {
                        let errors = {};
                        data.errors.map(error => {
                            errors[camelCase(error.key)] = error.errorMessage;
                        });

                        this.setState({ errors });
                    } else {
                        toast.error(data.message);
                    }
                }
            }).catch(axiosError);
        }
    }

    handleSelectTab = (key) => {
        this.setState({ activeTabKey: key });
    }

    renderRecordByProcess = (processId) => {
        let paProcesses = this.state.paProcesses.filter(p => p.processId === processId);
        return paProcesses.map(process => {
            if (process.performanceRecord) {
                const paRecord = {
                    typeId: 1,
                    roleId: process.processId,
                    creator: process.evaluator,
                    comment: process.performanceRecord.comment,
                    grade: process.performanceRecord.grade
                };
                return <PerformanceRecord data={paRecord} />;
            } else {
                return <span>No information</span>
            }
        });
    };

    render() {
        let paRecord = this.state.paRecord;
        let errors = this.state.errors;
        const selfRecord = this.renderRecordByProcess(PerformanceProcess.Self);
        const pmRecords = this.renderRecordByProcess(PerformanceProcess.ProjectManager);
        const evaluatorRecord = this.renderRecordByProcess(PerformanceProcess.Evaluator);
        const reviewerRecord = this.renderRecordByProcess(PerformanceProcess.Reviewer);
        return (
            <Modal keyboard={false} backdrop="static"
                show={this.props.isShowModal} onHide={this.props.onCloseModal}
                dialogClassName="custom-modal modal-md form-performance edit performance-appraisal"
            >
                <Modal.Header>
                    <Modal.Title>{getResource('EditPerformance')}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className="record record-your-mind">
                        <div className="comment">
                            <form className="form-filter form-performance edit">
                                <FormGroup>
                                    <ControlLabel className="required-label">{getResource('Comment')}</ControlLabel>
                                    <TextareaFullscreenEditor
                                        className={`form-control comment-field ${!isEmpty(errors) && errors.comment ? 'input-validation-error' : ''}`}
                                        name="comment"
                                        value={paRecord.comment}
                                        onChange={this.handleCommentChange}
                                    />
                                </FormGroup>
                                <FormGroup>
                                    <div className="grade">
                                        <ControlLabel>{getResource('Grade')}</ControlLabel>
                                        <Select name="grade" classNamePrefix="react-select"
                                            className={`react-select-container grade-field ${!isEmpty(errors) && errors.grade ? 'input-validation-error' : ''}`}
                                            placeholder="Select Grade"
                                            value={paRecord.grade}
                                            options={this.props.gradeOptions}
                                            isSearchable={false}
                                            onChange={this.handleGradeChange}
                                        />
                                    </div>
                                </FormGroup>
                            </form>
                        </div>
                    </div>
                    <div>
                        <Tabs id="tabs" activeKey={this.state.activeTabKey} onSelect={this.handleSelectTab} animation={false}>
                            <Tab eventKey={1} title="Self">
                                {selfRecord}
                            </Tab>
                            <Tab eventKey={2} title="PM">
                                {pmRecords}
                            </Tab>
                            <Tab eventKey={3} title="Evaluator">
                                {evaluatorRecord}
                            </Tab>
                            <Tab eventKey={4} title="Reviewer">
                                {reviewerRecord}
                            </Tab>
                        </Tabs>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button className="btn-cancel" onClick={this.props.onCloseModal}>{getResource('Cancel')}</Button>
                    <Button type="submit" bsStyle="primary" name="save" onClick={this.handleSubmit}>{getResource('Submit')}</Button>
                </Modal.Footer>
            </Modal>
        );
    }
}